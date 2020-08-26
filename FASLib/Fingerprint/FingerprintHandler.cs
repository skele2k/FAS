using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FASLib.DataAccess;
using libzkfpcsharp;
using System.Windows.Forms;
using FASLib.Models;
using FASLib.Helpers;

namespace FASLib.Fingerprint
{
    public class FingerprintHandler
    {
        public FingerprintHandler()
        {
            InitializeDevice();
        }

        public event EventHandler<(StaffModel, AttendanceModel)> SuccessfullyAddedToDBEvent;
        public event EventHandler<string> FailedToAddToDBEvent;


        Thread captureThread = null;
        byte[] theChosenOne = new byte[2048]; // Complete fingerprint template when registering. It is later returned to either AddUserControl or EditUserControl

        const int REGISTER_FINGER_COUNT = 3;

        zkfp fpInstance = new zkfp();
        IntPtr devHandle = IntPtr.Zero;
        IntPtr dbHandle = IntPtr.Zero;

        bool bIsTimeToDie = false;

        //bool IsRegister = false;
        //bool bIdentify = true;
        byte[] FPBuffer;
        int RegisterCount = 0;
        //int scanThreadID;

        //int deviceIndex;
        byte[][] RegTmps = new byte[REGISTER_FINGER_COUNT][];
        byte[][] fpData;

        int dataSize;

        byte[] RegTmp = new byte[2048];
        byte[] CapTmp = new byte[2048];
        int cbCapTmp = 2048;
        int regTempLen = 0;
        //int iFid = 1;

        const int MESSAGE_CAPTURED_OK = 0x0400 + 6;

        private int mfpWidth = 0;
        private int mfpHeight = 0;
        public byte[] GetTemplate()
        {
            return theChosenOne;
        }
        public bool InitializeDevice()
        {
            int callBackCode = fpInstance.Initialize();
            if (zkfp.ZKFP_ERR_OK == callBackCode)
            {
                int nCount = fpInstance.GetDeviceCount() - 1;
                if (nCount > 0)
                {
                    return true;
                }
                else
                {
                    fpInstance.CloseDevice();
                    return false;
                }
            }
            else
            {
                if (callBackCode == zkfperrdef.ZKFP_ERR_ALREADY_INIT)
                {
                    return true;
                }
                // Initialization failed
                return false;
            }
        }
        public void PushData(List<byte[]> fingerprints)
        {
            if (fingerprints == null)
            {
                return;
            }

            dataSize = fingerprints.Count();
            fpData = new byte[dataSize][];

            for (int i = 0; i < dataSize; i++)
            {
                fpData[i] = new byte[2048];
                fpData[i] = fingerprints[i];
            }
        }
        public void ConnectDeviceAndRegister()
        {
            int openDeviceCallBackCode = fpInstance.OpenDevice(0);

            if (zkfp.ZKFP_ERR_OK != openDeviceCallBackCode)
            {
                MessageBox.Show("Төхөөрөмжтэй холбогдоход алдаа гарлаа.");
                return;
            }

            for (int i = 0; i < REGISTER_FINGER_COUNT; i++)
            {
                RegTmps[i] = new byte[2048];
            }

            byte[] paramValue = new byte[4];

            // Retrieve the fingerprint image width
            int size = 4;
            fpInstance.GetParameters(1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            fpInstance.GetParameters(2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            // Create a thread to retrieve any new fingerprint and handle device events
            captureThread = new Thread(new ThreadStart(DoCaptureForRegister));
            captureThread.IsBackground = true;
            captureThread.Start();

            return;
        }

        /*private (bool isValid, AttendanceModel model) ValidateAttendanceForm()
        {
            AttendanceModel currentAttendance = new AttendanceModel();
            bool isValid = true;
            
        }*/
        public int GetNumOfTry()
        {
            return REGISTER_FINGER_COUNT - RegisterCount;
        }
        private double CalculateOfficeHours(string arriveTime, string leaveTime)
        {
            DateTime arrTime = DateTime.Parse(arriveTime);
            DateTime leaTime = DateTime.Parse(leaveTime);

            double hours = leaTime.Subtract(arrTime).TotalHours;
            return hours;
        }
        private void RegisterTimeToDB(byte[] fingerPrint)
        {
            StaffModel theStaff = null;
            try
            {
                var t = Task.Run(async () => await ApiProcessor.LoadStaffs());
                var staffList = t.Result;

                foreach (StaffModel staff in staffList)
                {
                    if (staff.fingerPrint.SequenceEqual(fingerPrint))
                    {
                        theStaff = staff;
                        break;
                    }
                }

                if (theStaff == null)
                {
                    FailedToAddToDBEvent?.Invoke(this, "");
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                AttendanceModel theRecordOfStaff = getRecordOfTheStaff(theStaff);
                bool atOffice = (theRecordOfStaff.atOffice > 0);

                if (atOffice == false && theRecordOfStaff.arriveTime == null)
                {
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    
                    AttendanceModel model = new AttendanceModel();
                    model.arriveTime = time;
                    model.atOffice = 1;
                    model.staff_id = theStaff.id;
                    model.branch_id = theStaff.branch_id;
                    model.date = date;

                    var res = Task.Run(async () => await ApiProcessor.EditAttendanceSheet(model));
                    var ans = res.Result;

                    SuccessfullyAddedToDBEvent?.Invoke(this,(theStaff, model));

                    //MessageBox.Show($"Сайн байна уу? {theStaff.fullName}");

                }
                else if (atOffice == false && theRecordOfStaff.arriveTime != null)
                {
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    
                    AttendanceModel model = new AttendanceModel();
                    model.atOffice = 1;
                    model.staff_id = theStaff.id;
                    model.branch_id = theStaff.branch_id;
                    model.date = date;

                    var res = Task.Run(async () => await ApiProcessor.EditAttendanceSheet(model));
                    var ans = res.Result;

                    model.arriveTime = time;
                    SuccessfullyAddedToDBEvent?.Invoke(this, (theStaff, model));

                    //MessageBox.Show($"Сайн байна уу? {theStaff.fullName}");
                }
                else if (atOffice == true && theRecordOfStaff.leaveTime == null)
                {
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    double officeHours = CalculateOfficeHours(theRecordOfStaff.arriveTime, time);

                    AttendanceModel model = new AttendanceModel();
                    model.leaveTime = time;
                    model.atOffice = 0;
                    model.staff_id = theStaff.id;
                    model.branch_id = theStaff.branch_id;
                    model.date = date;
                    model.officeHours = officeHours;

                    var res = Task.Run(async () => await ApiProcessor.EditAttendanceSheet(model));
                    var str = res.Result;

                    SuccessfullyAddedToDBEvent?.Invoke(this, (theStaff, model));

                    //MessageBox.Show($"Баяртай! {theStaff.fullName}");
                }
                else if (atOffice == true && theRecordOfStaff.leaveTime != null)
                {
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    double officeHours = CalculateOfficeHours(theRecordOfStaff.arriveTime, time);
                    
                    AttendanceModel model = new AttendanceModel();
                    model.leaveTime = time;
                    model.atOffice = 0;
                    model.staff_id = theStaff.id;
                    model.branch_id = theStaff.branch_id;
                    model.date = date;
                    model.officeHours = officeHours;

                    var res = Task.Run(async () => await ApiProcessor.EditAttendanceSheet(model));
                    var str = res.Result;

                    SuccessfullyAddedToDBEvent?.Invoke(this, (theStaff, model));

                    //MessageBox.Show($"Баяртай! {theStaff.fullName}");
                }
            }
            catch
            {
                
            }

        }

        private AttendanceModel getRecordOfTheStaff(StaffModel theStaff)
        {
            try
            {
                string currentDate = getCurrentDate();

                AttendanceModel theRecord = null;

                var t = Task.Run(async () => await ApiProcessor.LoadAttendanceSheet());
                var attendance = t.Result;

                for (int i = attendance.Count - 1; i >= 0; i--)
                {
                    if (attendance[i].staff_id == theStaff.id && attendance[i].date == currentDate)
                    {
                        theRecord = attendance[i];
                        break;
                    }
                }

                return theRecord;
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа.");
                return null;
            }
        }
        private string getCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("MM/dd/yyyy");
            return currentDate;
        }

        private string getCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss tt");
        }
        public void ConnectDeviceAndIdentify()
        {
            int openDeviceCallBackCode = fpInstance.OpenDevice(0);

            if (zkfp.ZKFP_ERR_OK != openDeviceCallBackCode)
            {
                MessageBox.Show("Төхөөрөмжтэй холбогдоход алдаа гарлаа.");
                return;
            }

            if (IntPtr.Zero == (dbHandle = zkfp2.DBInit()))
            {
                MessageBox.Show("Дататай холбогдоход алдаа гарлаа.");
                zkfp2.CloseDevice(devHandle);
                devHandle = IntPtr.Zero;
                return;
            }

            for (int i = 0; i < REGISTER_FINGER_COUNT; i++)
            {
                RegTmps[i] = new byte[2048];
            }

            byte[] paramValue = new byte[4];

            // Retrieve the fingerprint image width
            int size = 4;
            fpInstance.GetParameters(1, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpWidth);

            size = 4;
            fpInstance.GetParameters(2, paramValue, ref size);
            zkfp2.ByteArray2Int(paramValue, ref mfpHeight);

            FPBuffer = new byte[mfpWidth * mfpHeight];

            // Create a thread to retrieve any new fingerprint and handle device events
            captureThread = new Thread(new ThreadStart(DoCaptureForIdentify));
            captureThread.IsBackground = true;
            captureThread.Start();

            return;

        }

        private void DoCaptureForIdentify()
        {
            try
            {
                while (!bIsTimeToDie)
                {
                    int ret = fpInstance.AcquireFingerprint(FPBuffer, CapTmp, ref cbCapTmp);

                    if (ret == zkfp.ZKFP_ERR_OK)
                    {
                        RandomFingerprintCase();
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unexpected error; ERROR CODE: 3", e.ToString());
            }
        }

        private void DoCaptureForRegister()
        {
            try
            {
                while (!bIsTimeToDie)
                {
                    int ret = fpInstance.AcquireFingerprint(FPBuffer, CapTmp, ref cbCapTmp);

                    if (ret == zkfp.ZKFP_ERR_OK)
                    {
                        RegistrationCase();
                    }
                    Thread.Sleep(100);
                }
            }
            catch { }
        }

        public void RegistrationCase()
        {
            for (int i = 0; i < dataSize; i++)
            {
                int ret = fpInstance.Match(CapTmp, fpData[i]);
                if (ret > 0)
                {
                    MessageBox.Show("Уучлаарай, бүртгэлтэй хурууны хээ байна.");
                    return;
                }
            }
            //Check if the user has entered the fingerprint 3 times or not
            if (RegisterCount > 0 && fpInstance.Match(CapTmp, RegTmps[RegisterCount - 1]) <= 0)
            {
                return;
            }
            Array.Copy(CapTmp, RegTmps[RegisterCount], cbCapTmp);

            RegisterCount++;

            if (RegisterCount >= REGISTER_FINGER_COUNT)
            {
                // Generate a fp template by the combination of 3 successfully fingerprint acquisition
                int returnValue = fpInstance.GenerateRegTemplate(RegTmps[0], RegTmps[1], RegTmps[2], RegTmp, ref regTempLen);
                if (zkfp.ZKFP_ERR_OK == returnValue)
                {
                    RegisterCount = 0;
                    
                    bIsTimeToDie = true;
                    theChosenOne = RegTmp;
                    return;
                }
                else
                {
                    // unable to enroll due to some reason.
                }
                return;
            }
            else
            {
                //requires 3 fingerprints to successfully enroll
                // ask for the same fingerprint 2 more times
                int remainingCont = REGISTER_FINGER_COUNT - RegisterCount;
            }
        }

        private void RandomFingerprintCase()
        {
            for (int i = 0; i < dataSize; i++)
            {
                int ret = fpInstance.Match(CapTmp, fpData[i]);
                if (ret > 0)
                {
                    RegisterTimeToDB(fpData[i]);
                    return;
                }
            }
            FailedToAddToDBEvent?.Invoke(this, "");
        }

        public void DisconnectDevice()
        {
            int result = fpInstance.CloseDevice();
            captureThread.Abort();

            if (result == zkfp.ZKFP_ERR_OK)
            {
                result = fpInstance.Finalize();
                if (result == zkfp.ZKFP_ERR_OK)
                {
                    fpInstance.Clear();
                }
            }
        }
    }
}

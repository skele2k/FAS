using libzkfpcsharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FASDesktopUI.Models;
using FASDesktopUI.DataAccess;
namespace FASDesktopUI.Fingerprint
{
    class FingerprintHandler
    {
        public FingerprintHandler()
        {
            InitializeDevice();
        }

        Thread captureThread = null;
        string oneFP = string.Empty;
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
        Dictionary<byte[], string> mapper = new Dictionary<byte[], string>();
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
        int tickOrError = 0;
        // 1 = tick, 2 = error
        //private int mfpDpi = 0;
        public int TickOrError()
        {
            return tickOrError;
        }
        public string GetTemplate()
        {
            return oneFP;
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
                    MessageBox.Show("succ");
                    return true;
                }
                // Initialization failed
                return false;
            }
        }
        public void PushData(List<string> fingerprints)
        {
            dataSize = fingerprints.Count();
            fpData = new byte[dataSize][];

            for (int i = 0; i < dataSize; i++)
            {
                fpData[i] = new byte[2048];
                fpData[i] = zkfp.Base64String2Blob(fingerprints[i]);
                mapper[fpData[i]] = fingerprints[i];
            }
        }
        public string ConnectDeviceAndRegister()
        {
            int openDeviceCallBackCode = fpInstance.OpenDevice(0);

            if (zkfp.ZKFP_ERR_OK != openDeviceCallBackCode)
            {
                MessageBox.Show("Төхөөрөмжтэй холбогдоход алдаа гарлаа.");
                return "";
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

            return oneFP;
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
        public void StopThread()
        {
            bIsTimeToDie = true;
        }
        private double CalculateOfficeHours(string arriveTime, string leaveTime)
        {
            DateTime arrTime = DateTime.Parse(arriveTime);
            DateTime leaTime = DateTime.Parse(leaveTime);

            double hours = leaTime.Subtract(arrTime).TotalHours;
            return hours;
        }
        private void RegisterTimeToDB(string fingerPrint)
        {
            ObservableCollection<StaffModel> staffs = new ObservableCollection<StaffModel>();
            string sql = "SELECT * FROM staff WHERE fingerPrint = @fingerPrint";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@fingerPrint", fingerPrint}
            };

            var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, parameters);

            // just in case
            if (staffList.Count() == 0)
            {
                MessageBox.Show("Уучлаарай таны хурууны хээ бүртгэлгүй байна");
                return;
            }
            StaffModel theStaff = staffList[0];
            try
            {
                AttendanceModel theRecordOfStaff = getRecordOfTheStaff(theStaff);
                bool atOffice = (theRecordOfStaff.atOffice > 0);

                if (atOffice == false && theRecordOfStaff.arriveTime == null)
                {
                    tickOrError = 1;
                    sql = "UPDATE attendance SET arriveTime = @arriveTime, atOffice = @atOffice WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    parameters = null;
                    parameters = new Dictionary<string, object>
                    {
                        {"@arriveTime", time},
                        {"@atOffice", 1 },
                        {"@staff_id", theStaff.id },
                        {"@branch_id", theStaff.branch_id },
                        {"@date", date}
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                    //MessageBox.Show($"Сайн байна уу? {theStaff.fullName}");

                }
                else if (atOffice == false && theRecordOfStaff.arriveTime != null)
                {
                    tickOrError = 1;
                    sql = "UPDATE attendance SET atOffice = @atOffice WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    parameters = null;
                    parameters = new Dictionary<string, object>
                    {
                        {"@atOffice", 1 },
                        {"@staff_id", theStaff.id },
                        {"@branch_id", theStaff.branch_id },
                        {"@date", date}
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                    //MessageBox.Show($"Сайн байна уу? {theStaff.fullName}");
                }
                else if (atOffice == true && theRecordOfStaff.leaveTime == null)
                {
                    tickOrError = 1;
                    sql = "UPDATE attendance SET leaveTime = @leaveTime, atOffice = @atOffice, officeHours = @officeHours WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    double officeHours = CalculateOfficeHours(theRecordOfStaff.arriveTime, time);
                    parameters = null;
                    parameters = new Dictionary<string, object>
                    {
                        {"@leaveTime", time},
                        {"@atOffice", 0 },
                        {"@staff_id", theStaff.id },
                        {"@branch_id", theStaff.branch_id },
                        {"@date", date},
                        {"@officeHours", officeHours }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                    //MessageBox.Show($"Баяртай! {theStaff.fullName}");
                }
                else if (atOffice == true && theRecordOfStaff.leaveTime != null)
                {
                    tickOrError = 1;
                    sql = "UPDATE attendance SET leaveTime = @leaveTime, atOffice = @atOffice, officeHours = @officeHours WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                    string date = getCurrentDate();
                    string time = getCurrentTime();
                    double officeHours = CalculateOfficeHours(theRecordOfStaff.arriveTime, time);
                    parameters = null;
                    parameters = new Dictionary<string, object>
                    {
                        {"@leaveTime", time},
                        {"@atOffice", 0 },
                        {"@staff_id", theStaff.id },
                        {"@branch_id", theStaff.branch_id },
                        {"@date", date},
                        {"@officeHours", officeHours }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                    //MessageBox.Show($"Баяртай! {theStaff.fullName}");
                }
            }
            catch { }

        }

        private AttendanceModel getRecordOfTheStaff(StaffModel theStaff)
        {
            string sql = "SELECT * FROM attendance WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
            string currentDate = getCurrentDate();
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@staff_id", theStaff.id },
                {"@branch_id", theStaff.branch_id },
                {"@date", currentDate }
            };
            var attendanceList = SqliteDataAccess.LoadData<AttendanceModel>(sql, parameters);
            if (attendanceList.Count() == 0)
            {
                MessageBox.Show("Unexpected error; ERROR CODE: 2");
                return null;
            }
            AttendanceModel theRecord = attendanceList[0];

            return theRecord;
        }
        private string getCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("dd/MM/yyyy");
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
                    tickOrError = 0;
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
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

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
                    // LOAD TEMPLATE TO MEMORY
                    //returnValue = fpInstance.AddRegTemplate(iFid, RegTmp);
                    string fingerPrintTemplate = string.Empty;
                    zkfp.Blob2Base64String(RegTmp, regTempLen, ref fingerPrintTemplate);

                    // fingerPrintTemplate holdes successfully enrolled fingerprint in base64

                    bIsTimeToDie = true;
                    oneFP = fingerPrintTemplate;


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
                    RegisterTimeToDB(mapper[fpData[i]]);
                    return;
                }
            }
            tickOrError = 2;
            // MessageBox.Show("Тийм хурууны хээ байхкүэ хө");
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

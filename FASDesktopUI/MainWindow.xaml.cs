using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using FASLib.DataAccess;
using FASLib.Models;
using FASLib.Fingerprint;
using FASLib.Helpers;
using Caliburn.Micro;
using System.Runtime.InteropServices;

namespace FASDesktopUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FingerprintHandler fp;
        List<byte[]> fps = new List<byte[]>();
        ObservableCollection<StaffModel> staffs = new ObservableCollection<StaffModel>();
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
            Authenticate();
            fp = new FingerprintHandler();
            GetFingerprintsFromDB();
            fp.ConnectDeviceAndIdentify();
            
            InitializeCurrentDate();
            tick.Visibility = Visibility.Hidden;
            error.Visibility = Visibility.Hidden;

            fp.SuccessfullyAddedToDBEvent += Fp_SuccessfullyAddedToDBEvent;
            fp.FailedToAddToDBEvent += Fp_FailedToAddToDBEvent;
        }

        private void Fp_FailedToAddToDBEvent(object sender, string e)
        {
            this.Dispatcher.Invoke(() => { error.Visibility = Visibility.Visible; });
            Thread.Sleep(800);
            this.Dispatcher.Invoke(() => { error.Visibility = Visibility.Hidden; });
        }

        private void Fp_SuccessfullyAddedToDBEvent(object sender, (StaffModel, AttendanceModel) e)
        {
            this.Dispatcher.Invoke(() => { tick.Visibility = Visibility.Visible; });
            Thread.Sleep(800);
            this.Dispatcher.Invoke(() => { tick.Visibility = Visibility.Hidden; });
        }

        private void Authenticate()
        {
            try
            {
                var ret = Task.Run(async () => await ApiProcessor.Authenticate("FINGERPRINT1", "FINGERPRINT1"));
                Token token = ret.Result;

                if (token != null)
                {
                    ApiHelper.ApiClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token.Access_Token);
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа гарлаа. Сүлжээгээ шалгана уу?");
            }
        }

        public async void GetFingerprintsFromDB()
        {
            try
            {
                var staffList = await ApiProcessor.LoadStaffs();

                foreach (var model in staffList)
                {
                    if (model.fingerPrint != null)
                    {
                        fps.Add(model.fingerPrint);
                    }
                }
                fp.PushData(fps);
            }
            catch (Exception)
            {
                MessageBox.Show("Сүлжээтэй холбогдоход алдаа гарлаа.");
            }
        }
        private bool IsNewDay(string currentDate)
        {
            //string sql = "SELECT date FROM attendance WHERE date = @date";

            //Dictionary<string, object> parameters = new Dictionary<string, object>
            //{
            //    {"@date", currentDate }
            //};

            //var lastDateList = SqliteDataAccess.LoadData<string>(sql, parameters);
            var w = Task.Run(async () => await ApiProcessor.LoadAttendanceSheet());
            var attendanceList = w.Result;
            int size = attendanceList.Count();
            if (size == 0)
            {
                return true;
            }
            else
            {
                string lastDate = attendanceList[size - 1].date;
                return !(lastDate == currentDate);
            }
        }
        private AttendanceModel ValidateForm(StaffModel staffModel, string currentDate)
        {
            AttendanceModel model = new AttendanceModel();
            
            model.staff_id = staffModel.id;
            model.branch_id = staffModel.branch_id;
            model.date = currentDate;

            return model;
        }
        private void InitializeCurrentDate()
        {
            try
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                string currentDate = dateTime.ToString("MM/dd/yyyy");

                int numOfStaffs = CountStaff();
                bool isNewDay = IsNewDay(currentDate);
                if (isNewDay == true)
                {
                    for (int i = 0; i < numOfStaffs; i++)
                    {
                        StaffModel model = staffs[i];

                        var t = Task.Run(async () => await ApiProcessor.SaveToAttendanceSheet(ValidateForm(model, currentDate)));
                    }
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа.");
            }
        }

        private int CountStaff()
        {
            try
            {
                var t = Task.Run(async () => await ApiProcessor.LoadStaffs());
                var staffList = t.Result;
                staffList.ForEach(x => staffs.Add(x));
                return staffList.Count();
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа.");
                return 0;
            }
        }
    }

}


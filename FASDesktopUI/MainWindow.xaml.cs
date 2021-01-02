using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using FASLib.Models;
using FASLib.Fingerprint;
using FASLib.Helpers;
using System.Configuration;
using System.Windows.Threading;

namespace FASDesktopUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FingerprintHandler fp = null;
        List<byte[]> fps = new List<byte[]>();
        ObservableCollection<StaffModel> staffs = new ObservableCollection<StaffModel>();
        Dictionary<int, string> idBranchMap = new Dictionary<int, string>();
        public MainWindow()
        {
            InitializeComponent();
            ShowDefaultScreen();
        }
        private void InitializeFP()
        {
            fp = new FingerprintHandler();

            fp.SuccessfullyAddedToDBEvent += Fp_SuccessfullyAddedToDBEvent;
            fp.FailedToAddToDBEvent += Fp_FailedToAddToDBEvent;
        }
        private void disableIpButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                ipButton.IsEnabled = false;
            });
        }
        private void enableIpButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                ipButton.IsEnabled = true;
            });
        }
        private void StartUp()
        {
            Authenticate();
            GetFingerprintsFromDB();
            fp.ConnectDeviceAndIdentify();

            InitializeCurrentDate();
            this.Dispatcher.Invoke(() =>
            {
                userInfoStackPanel.Visibility = Visibility.Hidden;
                tick.Visibility = Visibility.Hidden;
                error.Visibility = Visibility.Hidden;
            });
            
        }
        private bool StartApp()
        {
            var api = Properties.Settings.Default.api;
            if (!ApiHelper.InitializeClient(api))
            {
                return false;
            }
            if (fp == null)
            {
                InitializeFP();
            }
            StartUp();
            LoadBranches();
            return true;
        }

        private void Fp_FailedToAddToDBEvent(object sender, string e)
        {
            this.Dispatcher.Invoke(() =>
            {
                userInfoStackPanel.Visibility = Visibility.Hidden;
                error.Visibility = Visibility.Visible;
            });
            Thread.Sleep(800);
            this.Dispatcher.Invoke(() => 
            { 
                error.Visibility = Visibility.Hidden;
            });
        }

        private void Fp_SuccessfullyAddedToDBEvent(object sender, (StaffModel, AttendanceModel) e)
        {
            this.Dispatcher.Invoke(() => {
                arriveTimeDisplayStackPanel.Visibility = Visibility.Hidden;
                leaveTimeDisplayStackPanel.Visibility = Visibility.Hidden;
                userInfoStackPanel.Visibility = Visibility.Visible;

                staffFirstNameTextBlock.Text = e.Item1.firstName;
                staffLastNameTextBlock.Text = e.Item1.lastName;
                branchNameTextBlock.Text = idBranchMap[e.Item1.branch_id];
                
                if (e.Item2.atOffice == 0)
                {
                    
                    leaveTimeTextBlock.Text = e.Item2.leaveTime;
                    arriveTimeDisplayStackPanel.Visibility = Visibility.Collapsed;
                    leaveTimeDisplayStackPanel.Visibility = Visibility.Visible;

                }
                else
                {
                    arriveTimeTextBlock.Text = e.Item2.arriveTime;
                    leaveTimeDisplayStackPanel.Visibility = Visibility.Collapsed;
                    arriveTimeDisplayStackPanel.Visibility = Visibility.Visible;
                }
                

                }); 
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
                    ApiHelper.ApiClient.DefaultRequestHeaders.Clear();
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
            catch { }
        }
        private bool IsNewDay(string currentDate)
        {
            //string sql = "SELECT date FROM attendance WHERE date = @date";

            //Dictionary<string, object> parameters = new Dictionary<string, object>
            //{
            //    {"@date", currentDate }
            //};

            //var lastDateList = SqliteDataAccess.LoadData<string>(sql, parameters);
            try
            {
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
            catch { return false; }
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
            catch { }
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
            catch { return 0; }
        }
        private void LoadBranches()
        {
            try
            {
                var t = Task.Run(async () => await ApiProcessor.LoadBranches());
                var branches = t.Result;

                for (int i = 0; i < branches.Count(); i++)
                {
                    idBranchMap[branches[i].id] = branches[i].name;
                }
            }
            catch { }
        }
        private async void ipButton_Click(object sender, RoutedEventArgs e)
        {
            if (fp != null)
            {
                fp.DisconnectDevice();
                fp = null;
            }
            var w = new getIpWindow();

            if (w.ShowDialog() == true)
            {
                if (w.IsSuccessful == true)
                {
                    disableIpButton();
                    var t = await Task.Run(() => StartApp());
                    if (t == false)
                    {
                        ShowDefaultScreen();
                    }
                    enableIpButton();
                }
            }
        }

        private void ShowDefaultScreen()
        {
            this.Dispatcher.Invoke(() => {
                staffLastNameTextBlock.Text = "------------";
                staffFirstNameTextBlock.Text = "------------";
                branchNameTextBlock.Text = "--------------";
                arriveTimeTextBlock.Text = "00:00:00 00";
                leaveTimeTextBlock.Text = "00:00:00 00";

                userInfoStackPanel.Visibility = Visibility.Visible;
                nameStackPanel.Visibility = Visibility.Visible;
                branchStackPanel.Visibility = Visibility.Visible;
                timeDisplayStackPanel.Visibility = Visibility.Visible;
                arriveTimeTextBlock.Visibility = Visibility.Visible;
                leaveTimeTextBlock.Visibility = Visibility.Visible;
                repeatFingerprintAlert.Visibility = Visibility.Hidden;
                error.Visibility = Visibility.Visible;
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, (object s, EventArgs ev) =>
            {
                this.myDateTime.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
            }, this.Dispatcher);
            timer.Start();
        }
    }

}


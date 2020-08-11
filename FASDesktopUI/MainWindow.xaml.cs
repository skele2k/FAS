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
            fp = new FingerprintHandler();
            GetFingerprintsFromDB();
            fp.PushData(fps);
            fp.ConnectDeviceAndIdentify();
            InitializeCurrentDate();
            tick.Visibility = Visibility.Hidden;
            error.Visibility = Visibility.Hidden;

            Thread tickOrErrorDisplayer = new Thread(new ThreadStart(displayTickOrError));
            tickOrErrorDisplayer.IsBackground = true;
            tickOrErrorDisplayer.Start();
        }
        public void GetFingerprintsFromDB()
        {
            try
            {
                string sql = "SELECT * FROM staff";
                var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());

                foreach (var model in staffList)
                {
                    if (model.fingerPrint != null)
                    {
                        fps.Add(model.fingerPrint);
                    }
                }
            }
            catch (Exception)
            {
                SqliteBaseRepository.CreateDatabase();
                string sql = "SELECT * FROM staff";
                var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());

                foreach (var model in staffList)
                {
                    if (model.fingerPrint != null)
                    {
                        fps.Add(model.fingerPrint);
                    }
                }
            }
        }
        private bool IsNewDay(string currentDate)
        {
            string sql = "SELECT date FROM attendance WHERE date = @date";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@date", currentDate }
            };

            var lastDateList = SqliteDataAccess.LoadData<string>(sql, parameters);
            int size = lastDateList.Count();
            if (size == 0)
            {
                return true;
            }
            else
            {
                string lastDate = lastDateList[size - 1];
                return !(lastDate == currentDate);
            }
        }

        private void InitializeCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("dd/MM/yyyy");

            int numOfStaffs = CountStaff();

            bool isNewDay = IsNewDay(currentDate);
            if (isNewDay == true)
            {
                string sql = "INSERT INTO attendance(staff_id, branch_id, date) VALUES(@staff_id, @branch_id, @date)";

                for (int i = 0; i < numOfStaffs; i++)
                {
                    StaffModel model = staffs[i];

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"@staff_id", model.id },
                        {"@branch_id", model.branch_id },
                        {"@date", currentDate }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                }
            }
        }

        private int CountStaff()
        {
            string sql = "SELECT * FROM staff";
            var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());
            staffList.ForEach(x => staffs.Add(x));
            return staffList.Count();
        }
        // Seperate thread for displaying tick or error. 
        private void displayTickOrError()
        {
            int code = fp.TickOrError();
            while (true)
            {
                if (code == 1)
                {
                    // This is when the fingerprint has been recognized
                    this.Dispatcher.Invoke(() => { tick.Visibility = Visibility.Visible; });
                    Thread.Sleep(1000);
                    this.Dispatcher.Invoke(() => { tick.Visibility = Visibility.Hidden; });
                }
                else if (code == 2)
                {
                    // This is when the fingerprint has not been recognized
                    this.Dispatcher.Invoke(() => { error.Visibility = Visibility.Visible; });
                    Thread.Sleep(1000);
                    this.Dispatcher.Invoke(() => { error.Visibility = Visibility.Hidden; });
                }
                if (fp == null)
                {
                    break;
                }
                code = fp.TickOrError();
            }
        }
    }

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Threading;
using FASLib.Fingerprint;
using FASLib.DataAccess;
using FASLib.Models;

namespace FASTAdmin
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        List<string> fps = new List<string>();
        FingerprintHandler fp;
        string fpTemplate;
        public string ReturnFP
        {
            get
            {
                return fpTemplate;
            }
        }
        public ScanWindow()
        {
            InitializeComponent();
            fp = new FingerprintHandler();
            GetFingerprintsFromDB();
            fp.PushData(fps);
            fp.ConnectDeviceAndRegister();

            Thread displayThread = new Thread(new ThreadStart(UpdateNumber));
            displayThread.IsBackground = true;
            displayThread.Start();
        }
        public void GetFingerprintsFromDB()
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

        private void UpdateNumber()
        {
            bool firstTwo = false;
            bool firstOne = false;
            int numOfTry = fp.GetNumOfTry();
            while (numOfTry > 0)
            {
                if (numOfTry == 2 && !firstTwo)
                {
                    this.Dispatcher.Invoke(() => { FINGER_SCAN_COUNT.Text = 2.ToString(); });
                    firstTwo = true;
                }
                else if (numOfTry == 1 && !firstOne)
                {
                    this.Dispatcher.Invoke(() => { FINGER_SCAN_COUNT.Text = 1.ToString(); });
                    firstOne = true;
                }
                numOfTry = fp.GetNumOfTry();
            }
            Thread.Sleep(100);
            fpTemplate = fp.GetTemplate();
            fp.DisconnectDevice();
            this.Dispatcher.Invoke(() => { Window.GetWindow(this).DialogResult = true; });
            this.Dispatcher.Invoke(() => { Window.GetWindow(this).Close(); });
        }
        
    }
}

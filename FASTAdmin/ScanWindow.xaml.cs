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
using FASLib.Helpers;
using System.ComponentModel;

namespace FASTAdmin
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        List<byte[]> fps = new List<byte[]>();
        FingerprintHandler fp;
        byte[] fpTemplate = new byte[2048];
        public byte[] ReturnFP
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

            fp.SuccessfullyScannedFP += Fp_SuccessfullyScannedFP;
            fp.GetTemplate += Fp_GetTemplate;
        }

        private void Fp_GetTemplate(object sender, byte[] e)
        {
            fpTemplate = e;
            Task.Run(() => CloseWindow());
        }

        private void Fp_SuccessfullyScannedFP(object sender, int e)
        {
            if (e != 0)
            {
                this.Dispatcher.Invoke(() => { FINGER_SCAN_COUNT.Text = e.ToString(); });
            }
        }

        private async Task GetFingerprintsFromDB()
        {
            var staffList = await ApiProcessor.LoadStaffs(); 
            if (staffList == null)
            {
                return;
            }
            foreach (var model in staffList)
            {
                if (model.fingerPrint != null)
                {
                    fps.Add(model.fingerPrint);
                }
            }
        }

        private async Task CloseWindow()
        {
            Thread.Sleep(200);
            fp.DisconnectDevice();
            this.Dispatcher.Invoke(() => { Window.GetWindow(this).DialogResult = true; });
            this.Dispatcher.Invoke(() => { Window.GetWindow(this).Close(); });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetFingerprintsFromDB();
            fp.PushData(fps);
            fp.ConnectDeviceAndRegister();

        }
    }
}

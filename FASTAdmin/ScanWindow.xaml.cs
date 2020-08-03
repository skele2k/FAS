﻿using System;
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

namespace FASTAdmin
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        List<byte[]> fps = new List<byte[]>();
        FingerprintHandler fp;
        byte[] fpTemplate;
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await GetFingerprintsFromDB();
            fp.PushData(fps);
            fp.ConnectDeviceAndRegister();

            Thread displayThread = new Thread(new ThreadStart(UpdateNumber));
            displayThread.IsBackground = true;
            displayThread.Start();
        }
    }
}
using FASLib.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace FASDesktopUI
{
    /// <summary>
    /// Interaction logic for getIpWindow.xaml
    /// </summary>
    public partial class getIpWindow : Window
    {
        bool isSuccessful = false;
        public getIpWindow()
        {
            InitializeComponent();

            InitializeIPAddress();
        }
        public bool IsSuccessful
        {
            get
            {
                return isSuccessful;
            }
        }
        private void InitializeIPAddress()
        {
            var api = ConfigurationManager.AppSettings["api"];
            ipInputTextBox.Text = CutAddress(api);
        }
        private string CutAddress(string api)
        {
            bool isSecond = false;
            int start = 0;
            int end = 0;
            for (int i = 0; i < api.Length; i++)
            {
                if (api[i] == ':')
                {
                    if (isSecond == false)
                    {
                        start = i + 3;
                        isSecond = true;

                    }
                    else
                    {
                        end = i;
                    }
                }
            }
            if (start != 0 && end != 0)
            {
                api = api.Substring(start, end - start);
            }
            return api;
        }
        private string CreateAddress(string api)
        {
            if (!api.StartsWith("http://") && !api.EndsWith(":8888"))
            {
                api = "http://" + api + ":8888";
            }
            return api;
        }
        private void submitIpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ipInputTextBox.Text == "" || ipInputTextBox.Text == "notset")
            {
                return;
            }
            string new_api = CreateAddress(ipInputTextBox.Text);

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["api"].Value = new_api;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
            isSuccessful = true;
            DialogResult = true;
            this.Close();
        }
    }
}

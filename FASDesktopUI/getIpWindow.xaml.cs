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
            if (api.StartsWith("https://") && api.EndsWith(":44360"))
            {
                int len = api.Length;
                api = api.Substring(8, len - 14);
            }
            return api;
        }
        private string CreateAddress(string api)
        {
            if (!api.StartsWith("https://") && !api.EndsWith(":44360"))
            {
                api = "https://" + api + ":44360";
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

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
        public getIpWindow()
        {
            InitializeComponent();

            InitializeIPAddress();
        }
        private void InitializeIPAddress()
        {
            var api = ConfigurationManager.AppSettings["api"];
            ipInputTextBox.Text = api;
        }

        private void submitIpButton_Click(object sender, RoutedEventArgs e)
        {
            if (ipInputTextBox.Text == "" || ipInputTextBox.Text == "notset")
            {
                return;
            }
            string new_api = ipInputTextBox.Text;

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["api"].Value = new_api;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");

            this.Close();
        }
    }
}

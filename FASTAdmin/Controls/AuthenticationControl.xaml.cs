using FASLib.Helpers;
using FASLib.Models;
using OfficeOpenXml.Packaging.Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
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

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for AuthenticationControl.xaml
    /// </summary>
    public partial class AuthenticationControl : UserControl
    {
        public AuthenticationControl()
        {
            InitializeComponent();

            InitializeIPAddress();
        }
        private void InitializeIPAddress()
        {
            var api = ConfigurationManager.AppSettings["api"];
            if (api != "notset")
            {
                ipAddressTextBox.Text = api;
            }
        }
        private (bool isValid, AdminModel model) ValidateForm()
        {
            AdminModel model = new AdminModel();
            bool isValid = true;
            try
            {
                model.username = addUsernameTextBox.Text;
                model.password = addPasswordTextBox.Password;
            }
            catch
            {
                isValid = false;
            }
            return (isValid, model);
        }
        private bool ConfigureAPI()
        {
            var api = ConfigurationManager.AppSettings["api"];
            var new_api = ipAddressTextBox.Text;
            if (new_api == "" && api == "notset" )
            {
                MessageBox.Show("Холбогдох сүлжээгээ өгнө үү?");
                return false;
            }

            if (new_api != api && new_api != "notset")
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["api"].Value = new_api;
                configuration.Save();

                ConfigurationManager.RefreshSection("appSettings");
            }
            ApiHelper.InitializeClient();
            return true;
        }
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (addUsernameTextBox.Text == "" || addPasswordTextBox.Password == "")
            {
                MessageBox.Show("Нэр эсвэл нууц үг хоосон байна.");
                return;
            }
            if(!ConfigureAPI())
            {
                return;
            }

            var form = ValidateForm();

            try
            {
                Token token = await ApiProcessor.Authenticate(form.model.username, form.model.password);

                if (token != null)
                {
                    loginTextBlock.Visibility = Visibility.Collapsed;
                    usernameStackPanel.Visibility = Visibility.Collapsed;
                    passwordStackPanel.Visibility = Visibility.Collapsed;
                    loginButton.Visibility = Visibility.Collapsed;
                    ipAddressStackPanel.Visibility = Visibility.Collapsed;
                    changePassTextBlock.Visibility = Visibility.Collapsed;

                    ApiHelper.ApiClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token.Access_Token);
                    BackControl.Content = new AdminControl();
                }
                else
                {
                    MessageBox.Show("Нэр эсвэл нууц үг буруу байна.");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Сүлжээнд холбогдсон эсэхээ шалгана уу?");
                return;
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfigureAPI())
            {
                return;
            }

            loginTextBlock.Visibility = Visibility.Collapsed;
            usernameStackPanel.Visibility = Visibility.Collapsed;
            passwordStackPanel.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Collapsed;
            ipAddressStackPanel.Visibility = Visibility.Collapsed;
            changePassTextBlock.Visibility = Visibility.Collapsed;

            BackControl.Content = new ChangePasswordControl();
        }
    }
}

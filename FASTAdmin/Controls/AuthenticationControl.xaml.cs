using FASLib.Helpers;
using FASLib.Models;
using FASLib.DataAccess;
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
            var api = Properties.Settings.Default.api;
            if (api != "notset")
            {
                ipAddressTextBox.Text = CutAddress(api);
            }
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
            var api = Properties.Settings.Default.api;
            
            string text = "";
            this.Dispatcher.Invoke(() =>
            {
                text = ipAddressTextBox.Text;
            });

            var new_api = CreateAddress(text); ;
            
            if (new_api == "" && api == "notset")
            {
                MessageBox.Show("Холбогдох сүлжээгээ өгнө үү?");
                return false;
            }
                
            if (new_api != api && new_api != "notset")
            {
                Properties.Settings.Default.api = new_api;
                Properties.Settings.Default.Save();
            }
            
            if (!ApiHelper.InitializeClient(new_api))
            {
                return false;
            }
            
            return true;
        
        } 
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loginButton.IsEnabled = false;
                if (addUsernameTextBox.Text == "" || addPasswordTextBox.Password == "")
                {
                    MessageBox.Show("Нэр эсвэл нууц үг хоосон байна.");
                    loginButton.IsEnabled = true;
                    return;
                }
                var asyncRun = await Task.Run(() => ConfigureAPI());
                if (!asyncRun)
                {
                    loginButton.IsEnabled = true;
                    return;
                }

                var form = ValidateForm();

                if (form.isValid == false)
                {
                    return;
                }
                Token token = await Task.Run(() => ApiProcessor.Authenticate(form.model.username, form.model.password));

                if (token != null)
                {
                    loginTextBlock.Visibility = Visibility.Collapsed;
                    usernameStackPanel.Visibility = Visibility.Collapsed;
                    passwordStackPanel.Visibility = Visibility.Collapsed;
                    loginButton.Visibility = Visibility.Collapsed;
                    ipAddressStackPanel.Visibility = Visibility.Collapsed;
                    changePassTextBlock.Visibility = Visibility.Collapsed;

                    ApiHelper.ApiClient.DefaultRequestHeaders.Clear();
                    ApiHelper.ApiClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token.Access_Token);
                    BackControl.Content = new AdminControl();
                }
                else
                {
                    MessageBox.Show("Нэр эсвэл нууц үг буруу байна.");
                }
            }
            catch (Exception)
            {
            }
            loginButton.IsEnabled = true;
        }

        private async void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var asyncRun = await Task.Run(() => ConfigureAPI());
            if (!asyncRun)
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

using FASLib.Helpers;
using FASLib.Models;
using System;
using System.Collections.Generic;
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
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (addUsernameTextBox.Text == "" || addPasswordTextBox.Password == "")
            {
                MessageBox.Show("Нэр эсвэл нууц үг хоосон байна.");
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
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
                return;
            }
        }
    }
}

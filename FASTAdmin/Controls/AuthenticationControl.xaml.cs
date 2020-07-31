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
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (addUsernameTextBox.Text == "" || addPasswordTextBox.Password == "")
            {
                MessageBox.Show("Нэр эсвэл нууц үг хоосон байна.");
                return;
            }
            var form = ValidateForm();

            if (form.model.username == "admin" && form.model.password == "admin")
            {
                loginTextBlock.Visibility = Visibility.Collapsed;
                usernameStackPanel.Visibility = Visibility.Collapsed;
                passwordStackPanel.Visibility = Visibility.Collapsed;
                loginButton.Visibility = Visibility.Collapsed;
                BackControl.Content = new AdminControl();
            }
            else
            {
                MessageBox.Show("Нэр эсвэл нууц үг буруу байна.");
                return;
            }
        }
    }
}

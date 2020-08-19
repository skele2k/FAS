using FASLib.Helpers;
using FASLib.Models;
using OfficeOpenXml.Utils;
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
    /// Interaction logic for ChangePasswordControl.xaml
    /// </summary>
    public partial class ChangePasswordControl : UserControl
    {
        public ChangePasswordControl()
        {
            InitializeComponent();
        }
        private (bool isValid, AdminModel model) ValidateNewForm()
        {
            if (addPasswordTextBox.Password != passwordRepeatTextBox.Password)
            {
                MessageBox.Show("Шинэ нууц үг таарсангүй.");
                return (false, null);
            }
            if (addUsernameTextBox.Text == "")
            {
                return (false, null);
            }

            AdminModel model = new AdminModel();
            try
            {
                model.username = addUsernameTextBox.Text;
                model.password = addPasswordTextBox.Password;

                return (true, model);
            }
            catch
            {
                return (false, null);
            }
        }
        private (bool isValid, AdminModel model) ValidateOldForm()
        {
            if (addUsernameTextBox.Text == "" || addOldPasswordTextBox.Password == "")
            {
                MessageBox.Show("Нэр эсвэл нууц үг хоосон");
                return (false, null);
            }

            AdminModel model = new AdminModel();
            try
            {
                model.username = addUsernameTextBox.Text;
                model.password = addOldPasswordTextBox.Password;

                return (true, model);
            }
            catch
            {
                return (false, null);
            }
        }
        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            var form = ValidateNewForm();
            var oldform = ValidateOldForm();

            if (form.isValid == false || oldform.isValid == false)
            {
                return;
            }

            if (oldform.model.password == form.model.password)
            {
                MessageBox.Show("Хуучин нууц үгтэй адилхан нууц үг байж болохгүй.");
                return;
            }

            try
            {
                if (oldform.isValid)
                {
                    var t = Task.Run(async () => await ApiProcessor.Authenticate(oldform.model.username, oldform.model.password));
                    Token token = t.Result;

                    if (token != null)
                    {
                        ApiHelper.ApiClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token.Access_Token);
                    }
                    else
                    {
                        MessageBox.Show("Буруу нэр эсвэл нууц үг");
                        return;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээнд холбогдсон эсэхээ шалгана уу?");
                return;
            }

            if (form.isValid)
            {
                int theId = 0;
                try
                {
                    var t = Task.Run(async () => await ApiProcessor.LoadAdmins());
                    var admins = t.Result;

                    foreach(AdminModel admin in admins)
                    {
                        if (admin.username == form.model.username)
                        {
                            theId = admin.id;
                            break;
                        }
                    }
                    if (theId == 0)
                    {
                        throw new Exception();
                    }
                }
                catch { return; }

                try
                {
                    var t = Task.Run(async () => await ApiProcessor.EditAdminByID(theId, form.model));
                    var message = t.Result;
                    if (message == "success")
                    {
                        MessageBox.Show("Амжилттай.");

                        backButton.Visibility = Visibility.Collapsed;
                        passwordChangeTextBlock.Visibility = Visibility.Collapsed;
                        passwordRepeatStackPanel.Visibility = Visibility.Collapsed;
                        usernameStackPanel.Visibility = Visibility.Collapsed;
                        passwordStackPanel.Visibility = Visibility.Collapsed;
                        oldPasswordStackPanel.Visibility = Visibility.Collapsed;
                        changeButton.Visibility = Visibility.Collapsed;
                        backControl.Content = new AuthenticationControl();

                        return;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("Алдаа гарлаа.");
                    return;
                }
            }
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            backButton.Visibility = Visibility.Collapsed;
            passwordChangeTextBlock.Visibility = Visibility.Collapsed;
            passwordRepeatStackPanel.Visibility = Visibility.Collapsed;
            usernameStackPanel.Visibility = Visibility.Collapsed;
            passwordStackPanel.Visibility = Visibility.Collapsed;
            oldPasswordStackPanel.Visibility = Visibility.Collapsed;
            changeButton.Visibility = Visibility.Collapsed;
            backControl.Content = new AuthenticationControl();
        }
    }
}

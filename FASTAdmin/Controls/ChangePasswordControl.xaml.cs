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
        private void changeButton_Click(object sender, RoutedEventArgs e)
        {
            if (addPasswordTextBox.Password != passwordRepeatTextBox.Password)
            {
                MessageBox.Show("Шинэ нууц үг таарсангүй.");
            }
        }
    }
}

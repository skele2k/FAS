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
    /// Interaction logic for AdminControl.xaml
    /// </summary>
    public partial class AdminControl : UserControl
    {
        public AdminControl()
        {
            InitializeComponent();
        }

        private void exportToExcelMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addStaffMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new AddUserControl();
        }

        private void deleteStaffMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new DeleteUserControl();
        }

        private void editStaffMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new EditUserControl();
        }

        private void addBranchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new AddBranchControl();
        }

        private void deleteBranchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new DeleteBranchControl();
        }

        private void editBranchMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new EditBranchControl();
        }

        private void leaveAdminButton_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            leaveContent.Content = new AuthenticationControl();
        }
    }
}

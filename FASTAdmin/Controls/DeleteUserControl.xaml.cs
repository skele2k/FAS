using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using FASLib.Models;
using FASLib.DataAccess;
using FASLib.Helpers;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for DeleteUserControl.xaml
    /// </summary>
    public partial class DeleteUserControl : UserControl
    {
        ObservableCollection<StaffModel> staffs = new ObservableCollection<StaffModel>();
        public DeleteUserControl()
        {
            InitializeComponent();
            WireUpStaffDropdown();
        }
        private async Task InitializeStaffList()
        {
            var staffList = await ApiProcessor.LoadStaffs();
            staffList.ForEach(x => staffs.Add(x));
        }
        private void WireUpStaffDropdown()
        {
            staffSelectDropDown.ItemsSource = staffs;
            staffSelectDropDown.DisplayMemberPath = "fullName";
            staffSelectDropDown.SelectedValuePath = "id";
        }

        private async void deleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (staffSelectDropDown.SelectedItem == null)
            {
                MessageBox.Show("Устгах ажилчнаа сонгоно уу?");
                return;
            }

            StaffModel model = (StaffModel)staffSelectDropDown.SelectedItem;
            var t = await ApiProcessor.DeleteStaffByID(model.id);
            if (t == "success")
            {
                MessageBox.Show("Амжилттай устгалаа.");
            }
            else
            {
                MessageBox.Show("Устгалт амжилтгүй");
            }

            ResetForm();
            staffs.Clear();
            await InitializeStaffList();
            WireUpStaffDropdown();
        }
        private void ResetForm()
        {
            staffSelectDropDown.SelectedItem = null;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            deleteStaffTextBox.Visibility = Visibility.Collapsed;
            staffSelectStackPanel.Visibility = Visibility.Collapsed;
            deleteStaff.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeStaffList();
        }
    }
}

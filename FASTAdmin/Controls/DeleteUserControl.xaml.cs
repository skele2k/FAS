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
        public StaffModel selectedStaff { get; set; }
        public DeleteUserControl()
        {
            InitializeComponent();
            WireUpStaffDropdown();
        }
        private async Task InitializeStaffList()
        {
            try
            {
                var staffList = await ApiProcessor.LoadStaffs();
                staffList.ForEach(x => staffs.Add(x));
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
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

            try
            {
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
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
        }
        private void ResetForm()
        {
            deleteStaffTextBox.Visibility = Visibility.Collapsed;
            staffSelectStackPanel.Visibility = Visibility.Collapsed;
            deleteStaff.Visibility = Visibility.Collapsed;
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeStaffList();

            int left = 0;
            int right = staffs.Count() - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (staffs[mid].id == selectedStaff.id)
                {
                    staffSelectDropDown.SelectedItem = staffs[mid];
                    break;
                }
                else if (staffs[mid].id > selectedStaff.id)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
        }
    }
}

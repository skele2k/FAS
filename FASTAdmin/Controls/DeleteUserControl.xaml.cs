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
            InitializeStaffList();
            WireUpStaffDropdown();
        }
        private void InitializeStaffList()
        {
            string sql = "SELECT * FROM staff";
            var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());

            staffList.ForEach(x => staffs.Add(x));
        }
        private void WireUpStaffDropdown()
        {
            staffSelectDropDown.ItemsSource = staffs;
            staffSelectDropDown.DisplayMemberPath = "fullName";
            staffSelectDropDown.SelectedValuePath = "id";
        }

        private void deleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (staffSelectDropDown.SelectedItem == null)
            {
                MessageBox.Show("Устгах ажилчнаа сонгоно уу?");
                return;
            }

            StaffModel model = (StaffModel)staffSelectDropDown.SelectedItem;
            string sql = "DELETE FROM staff WHERE id = @id AND branch_id = @branch_id AND firstName = @firstName AND lastName = @lastName AND hasLunch = @hasLunch";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@id", model.id },
                {"@branch_id", model.branch_id },
                {"@firstName", model.firstName },
                {"@lastName", model.lastName },
                {"@hasLunch", model.hasLunch }
            };
            SqliteDataAccess.SaveData(sql, parameters);
            MessageBox.Show("Амжилттай устгалаа.");

            ResetForm();
            staffs.Clear();
            InitializeStaffList();
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
    }
}

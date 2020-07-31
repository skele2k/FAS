
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
using System.Threading;
using System.Collections.ObjectModel;
using FASLib.DataAccess;
using FASLib.Models;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for EditUserControl.xaml
    /// </summary>
    public partial class EditUserControl : UserControl
    {
        ObservableCollection<StaffModel> staffs = new ObservableCollection<StaffModel>();
        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        string fpTemplate = "";
        public EditUserControl()
        {
            InitializeComponent();
            InitializeStaffDropdown();
            WireUpStaffDropdown();
            InitializeBranchDropdown();
            WireUpBranchDropdown();
            ToggleFormFieldsDisplay(false);
            
        }
        private void InitializeBranchDropdown()
        {
            string sql = "SELECT * FROM branch";
            var branchList = SqliteDataAccess.LoadData<BranchModel>(sql, new Dictionary<string, object>());
            branchList.ForEach(x => branches.Add(x));
        }
        private void InitializeStaffDropdown()
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
        private void WireUpBranchDropdown()
        {
            newBranchSelectDropDown.ItemsSource = branches;
            newBranchSelectDropDown.DisplayMemberPath = "name";
            newBranchSelectDropDown.SelectedValuePath = "id";
        }

        private void LoadStaff()
        {
            if (staffSelectDropDown.SelectedItem != null)
            {
                StaffModel model = (StaffModel)staffSelectDropDown.SelectedItem;
                newFirstname.Text = model.firstName;
                newLastname.Text = model.lastName;
                lunchChangeCheckBox.IsChecked = (model.hasLunch > 0);
                newBranchSelectDropDown.SelectedValue = model.branch_id;
            }
        }
        private (bool isValid, StaffModel model) ValidateForm()
        {
            bool isValid = true;
            StaffModel model = new StaffModel();
            try
            {
                model.firstName = newFirstname.Text;
                model.lastName = newLastname.Text;
                model.hasLunch = (bool) lunchChangeCheckBox.IsChecked ? 1 : 0;
                model.id = (int) staffSelectDropDown.SelectedValue;
                model.branch_id = (int) newBranchSelectDropDown.SelectedValue;
            }
            catch
            {
                isValid = false;
            }
            return (isValid, model);
        }
        private void SaveStaffToDatabase()
        {
            var form = ValidateForm();
            if (form.isValid == false)
            {
                MessageBox.Show("Өгөгдлөө зөв бөглөнө үү?");
                return;
            }
            string sql = "UPDATE staff SET branch_id = @branch_id, firstName = @firstName, lastName = @LastName, hasLunch = @hasLunch, fingerPrint = @fingerPrint WHERE id = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@id", form.model.id },
                {"@branch_id", form.model.branch_id },
                {"@firstName", form.model.firstName },
                {"@lastName", form.model.lastName },
                {"@hasLunch", form.model.hasLunch },
                {"@fingerPrint", fpTemplate }
            };
            SqliteDataAccess.SaveData(sql, parameters);
        }
        private void ResetForm()
        {
            newFirstname.Text = "";
            newLastname.Text = "";
            lunchChangeCheckBox.IsChecked = false;
            staffSelectDropDown.SelectedItem = null;
            newBranchSelectDropDown.SelectedItem = null;
            newFpAddButton.Content = "Шинэ хурууны хээ таниулах";
            newFpAddButton.IsEnabled = true;
        }
        private void submitNewUser_Click(object sender, RoutedEventArgs e)
        {
            if (newFirstname.Text == "" || newLastname.Text == "")
            {
                MessageBox.Show("Нэр эсвэл Овог оруулна уу?");
                return;
            }
            SaveStaffToDatabase();
            MessageBox.Show("Амжилттай шинэчиллээ.");

            ResetForm();
            ToggleFormFieldsDisplay(false);
            staffs.Clear();
            InitializeStaffDropdown();
            WireUpStaffDropdown();

        }
        private void ToggleFormFieldsDisplay(bool displayFields)
        {
            Visibility display = displayFields ? Visibility.Visible : Visibility.Collapsed;

            newLastNameStackPanel.Visibility = display;
            newFirstNameStackPanel.Visibility = display;
            newBranchStackPanel.Visibility = display;
            newFpAddButton.Visibility = display;
            lunchChangeCheckBox.Visibility = display;
            submitNewUser.Visibility = display;
        }
        private void staffSelectDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadStaff();
            ToggleFormFieldsDisplay(true);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            editTextBlock.Visibility = Visibility.Collapsed;
            staffSelectStackPanel.Visibility = Visibility.Collapsed;
            newLastNameStackPanel.Visibility = Visibility.Collapsed;
            newFirstNameStackPanel.Visibility = Visibility.Collapsed;
            newBranchStackPanel.Visibility = Visibility.Collapsed;
            newFpAddButton.Visibility = Visibility.Collapsed;
            lunchChangeCheckBox.Visibility = Visibility.Collapsed;
            submitNewUser.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();
        }

        private void newFpAddButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ScanWindow();

            if (w.ShowDialog() == true)
            {
                fpTemplate = w.ReturnFP;
            }
            if (fpTemplate != "")
            {
                newFpAddButton.Content = "Хурууны хээ бүртгэгдсэн";
                newFpAddButton.IsEnabled = false;
            }
        }
    }
}

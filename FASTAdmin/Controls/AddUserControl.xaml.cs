using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FASLib.DataAccess;
using FASLib.Models;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for AddUserControl.xaml
    /// </summary>
    public partial class AddUserControl : UserControl
    {
        //List<StaffModel> staffs;
        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        string fpTemplate = "";
        public AddUserControl()
        {
            InitializeComponent();
            InitializeBranchList();
            WireUpBranchDropDown();
            
        }

        private void InitializeBranchList()
        {
            string sql = "SELECT * FROM branch";
            var branchesList = SqliteDataAccess.LoadData<BranchModel>(sql, new Dictionary<string, object>());

            branchesList.ForEach(x => branches.Add(x));
        }
        private int FindBranchID()
        {
            return (int)branchSelectDropdown.SelectedValue;
        }
        private void AddStaffsToDatabase()
        {
            var form = Validate();

            if (form.isValid == false)
            {
                MessageBox.Show("Форматаа зөв оруулна уу?");
                return;
            }

            string sql = "INSERT INTO staff(branch_id, firstName, lastName ,fingerPrint , hasLunch) " +
                         "VALUES(@branch_id, @firstName, @lastName ,@fingerPrint ,@hasLunch)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", form.model.branch_id },
                {"@firstName", form.model.firstName },
                {"@lastName", form.model.lastName },
                {"@fingerPrint", fpTemplate },
                {"@hasLunch", form.model.hasLunch }
            };
            SqliteDataAccess.SaveData(sql, parameters);
            InsertToAttendanceSheet();
            MessageBox.Show("Амжилттай ажилтан нэмлээ.");
        }
        private string getCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("dd/MM/yyyy");
            return currentDate;
        }

        private StaffModel GetStaff()
        {
            string sql = "SELECT * FROM staff WHERE fingerPrint = @fingerPrint";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@fingerPrint", fpTemplate }
            };
            var staffList = SqliteDataAccess.LoadData<StaffModel>(sql, parameters);
            if (staffList.Count() == 0)
            {
                throw new Exception();
            }
            return staffList[0];
        }

        private void InsertToAttendanceSheet()
        {
            string sql = "INSERT INTO attendance(staff_id, branch_id, date, hasLunch) VALUES (@staff_id, @branch_id, @date, @hasLunch)";
            string date = getCurrentDate();
            StaffModel model = GetStaff();
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@staff_id", model.id },
                {"@branch_id", model.branch_id },
                {"@date", date },
                {"@hasLunch", model.hasLunch }
            };
            SqliteDataAccess.SaveData(sql, parameters);
        }
/*        private void AddStaffsToList()
        {
            string sql = "SELECT * FROM staff";
            staffs = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());
        }*/

        private void WireUpBranchDropDown()
        {
            branchSelectDropdown.ItemsSource = branches;
            branchSelectDropdown.DisplayMemberPath = "name";
            branchSelectDropdown.SelectedValuePath = "id";
        }

        private void fpAddButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new ScanWindow();

            if (w.ShowDialog() == true)
            {
                fpTemplate = w.ReturnFP;
            }
            if (fpTemplate != "")
            {
                fpAddButton.Content = "Хурууны хээ бүртгэгдсэн";
                fpAddButton.IsEnabled = false;
            }
        }

        private (bool isValid, StaffModel model) Validate()
        {
            bool isValid = true;
            StaffModel model = new StaffModel();

            try
            {
                model.firstName = addFirstNameTextBox.Text;
                model.lastName = addLastNameTextBox.Text;
                model.hasLunch = (bool) hasLunch.IsChecked ? 1 : 0;

            }
            catch
            {
                isValid = false;
            }
            model.branch_id = FindBranchID();
            return (isValid, model);
        }
        private void addStaffButton_Click(object sender, RoutedEventArgs e)
        {
            if (addLastNameTextBox.Text == "" || addFirstNameTextBox.Text == "")
            {
                MessageBox.Show("Нэр эсвэл Овог хоосон байна.");
                return;
            }
            if (branchSelectDropdown.SelectedItem == null)
            {
                MessageBox.Show("Тасгаа сонгоно уу?");
                return;
            }
            if (fpTemplate == "")
            {
                MessageBox.Show("Хурууны хээгээ оруулна уу?");
                return;
            }

            AddStaffsToDatabase();
            ResetForm();
            branches.Clear();
            InitializeBranchList();
            WireUpBranchDropDown();
        }

        private void ResetForm()
        {
            addFirstNameTextBox.Text = "";
            addLastNameTextBox.Text = "";
            hasLunch.IsChecked = false;
            branchSelectDropdown.SelectedItem = null;
            fpAddButton.Content = "Хурууны хээ таниулах";
            fpAddButton.IsEnabled = true;

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            AddUser.Visibility = Visibility.Collapsed;
            lastnameStackPanel.Visibility = Visibility.Collapsed;
            firstnameStackPanel.Visibility = Visibility.Collapsed;
            branchStackPanel.Visibility = Visibility.Collapsed;
            fpAddButton.Visibility = Visibility.Collapsed;
            hasLunch.Visibility = Visibility.Collapsed;
            addStaffButton.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();
        }
    }
}

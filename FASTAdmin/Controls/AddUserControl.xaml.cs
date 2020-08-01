using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FASLib.DataAccess;
using FASLib.Helpers;
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
            WireUpBranchDropDown();
            
        }

        private async Task InitializeBranchList()
        {

            var branchesList = await ApiProcessor.LoadBranches();
            if (branchesList != null)
            {
                branchesList.ForEach(x => branches.Add(x));
            }
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
            
            var t = Task.Run(() => ApiProcessor.SaveStaff(form.model));
            t.Wait();

            InsertToAttendanceSheet(form.model);
            MessageBox.Show("Амжилттай ажилтан нэмлээ.");
        }
        private string getCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("dd/MM/yyyy");
            return currentDate;
        }
        private (AttendanceModel model, bool isValid) ValidateAttendanceModel(StaffModel theStaff)
        {
            AttendanceModel model = new AttendanceModel();
            bool isValid = true;

            try
            {
                model.staff_id = theStaff.id;
                model.branch_id = theStaff.branch_id;
                model.date = getCurrentDate();
                model.atOffice = 0;
                model.hasLunch = theStaff.hasLunch > 0;
            }
            catch
            {
                isValid = false;
            }
            return (model, isValid);
        }
        private async void InsertToAttendanceSheet(StaffModel theStaff)
        {
            var staffs = await ApiProcessor.LoadStaffs();
            foreach(StaffModel staff in staffs)
            {
                if (staff.fingerPrint == theStaff.fingerPrint)
                {
                    theStaff.id = staff.id;
                    break;
                }
            }
            var form = ValidateAttendanceModel(theStaff);

            var t = Task.Run(() => ApiProcessor.SaveToAttendanceSheet(form.model));
            t.Wait();
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
                model.fingerPrint = fpTemplate;
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
            //await InitializeBranchList();
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

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchList();
        }
    }
}

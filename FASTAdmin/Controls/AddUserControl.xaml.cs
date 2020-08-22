using FASLib.Fingerprint;
using FASLib.Helpers;
using FASLib.Models;
using libzkfpcsharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for AddUserControl.xaml
    /// </summary>
    public partial class AddUserControl : UserControl
    {
        public event EventHandler<string> UpdateDataGridEvent;

        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        byte[] fpTemplate;
        public AddUserControl()
        {
            InitializeComponent();
            WireUpBranchDropDown();
        }
        
        private async Task InitializeBranchList()
        {

            try
            {
                var branchesList = await ApiProcessor.LoadBranches();
                if (branchesList != null)
                {
                    branchesList.ForEach(x => branches.Add(x));
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
        }

        private int FindBranchID()
        {
            return (int)branchSelectDropdown.SelectedValue;
        }
        private async void AddStaffsToDatabase()
        {
            var form = Validate();

            if (form.isValid == false)
            {
                MessageBox.Show("Форматаа зөв оруулна уу?");
                return;
            }

            try
            {
                var t = await ApiProcessor.SaveStaff(form.model);
                if (t == "success")
                {
                    await InsertToAttendanceSheet(form.model);
                }
                else
                {
                    MessageBox.Show("Мэдээллийг хадгалахад алдаа гарлаа.");
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }

        }
        private string getCurrentDate()
        {
            DateTime dateTime = DateTime.UtcNow.Date;
            string currentDate = dateTime.ToString("MM/dd/yyyy");
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
            }
            catch
            {
                isValid = false;
            }
            return (model, isValid);
        }
        private async Task InsertToAttendanceSheet(StaffModel theStaff)
        {
            try
            {
                var staffs = await ApiProcessor.LoadStaffs();

                foreach (StaffModel staff in staffs)
                {
                    if (theStaff.fingerPrint.SequenceEqual(staff.fingerPrint))
                    {
                        theStaff.id = staff.id;
                        break;
                    }
                }
                var form = ValidateAttendanceModel(theStaff);

                var t = Task.Run(async () => await ApiProcessor.SaveToAttendanceSheet(form.model));
                var res = t.Result;
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
            
        }
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
            if (fpTemplate != null)
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
            if (fpTemplate == null)
            {
                MessageBox.Show("Хурууны хээгээ оруулна уу?");
                return;
            }

            AddStaffsToDatabase();
            ResetForm();
            branches.Clear();
            System.Threading.Thread.Sleep(100);
            UpdateDataGridEvent?.Invoke(this, "");
        }

        private void ResetForm()
        {
            AddUser.Visibility = Visibility.Collapsed;
            lastnameStackPanel.Visibility = Visibility.Collapsed;
            firstnameStackPanel.Visibility = Visibility.Collapsed;
            branchStackPanel.Visibility = Visibility.Collapsed;
            fpAddButton.Visibility = Visibility.Collapsed;
            addStaffButton.Visibility = Visibility.Collapsed;
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchList();
        }
    }
}

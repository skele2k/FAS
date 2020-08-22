using FASLib.Helpers;
using FASLib.Models;
using FASTAdmin.excelHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        BranchModel selectedBranch;
        StaffModel selectedStaff;
        public AdminControl()
        {
            InitializeComponent();
        }
        private async Task LoadDataGrid()
        {
            try
            {
                var staffs = await ApiProcessor.LoadStaffs();
                this.Dispatcher.Invoke(() => { staffDataGrid.ItemsSource = staffs; });

                var branches = await ApiProcessor.LoadBranches();
                this.Dispatcher.Invoke(() => { branchDataGrid.ItemsSource = branches; });
            }
            catch(Exception e)
            {
                //MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
                MessageBox.Show(e.ToString());
            }
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataGrid();
        }

        private void AddNewStaffButton_Click(object sender, RoutedEventArgs e)
        {
            AddUserControl t = new AddUserControl();
            externalContents.Content = t;
            t.UpdateDataGridEvent += T_UpdateDataGridEvent;
        }

        private void T_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void deleteSelectedItemButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (staffDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Устгах ажилтнаа сонгоно уу?");
                return;
            }

            selectedStaff = (StaffModel) staffDataGrid.SelectedItem;
            DeleteUserControl deleteControl = new DeleteUserControl();
            deleteControl.selectedStaff = selectedStaff;
            externalContents.Content = deleteControl;
            deleteControl.UpdateDataGridEvent += DeleteControl_UpdateDataGridEvent;
        }

        private void DeleteControl_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void EditSelectedItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (staffDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Янзлах ажилтнаа сонгоно уу?");
                return;
            }

            selectedStaff = (StaffModel)staffDataGrid.SelectedItem;
            EditUserControl editControl = new EditUserControl();
            editControl.selectedStaff = selectedStaff;
            externalContents.Content = editControl;
            editControl.UpdateDataGridEvent += EditControl_UpdateDataGridEvent;
        }

        private void EditControl_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void addNewBranchButton_Click(object sender, RoutedEventArgs e)
        {
            AddBranchControl addBranch = new AddBranchControl();
            externalContents.Content = addBranch;
            addBranch.UpdateDataGridEvent += AddBranch_UpdateDataGridEvent;
        }

        private void AddBranch_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void deleteSelectedBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (branchDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Устгах тасгаа сонгоно уу?");
                return;
            }

            selectedBranch = (BranchModel) branchDataGrid.SelectedItem;
            DeleteBranchControl deleteControl = new DeleteBranchControl();
            deleteControl.selectedBranch = selectedBranch;
            externalContents.Content = deleteControl;
            deleteControl.UpdateDataGridEvent += DeleteControl_UpdateDataGridEvent1;
        }

        private void DeleteControl_UpdateDataGridEvent1(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void EditSelectedBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (branchDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Янзлах тасгаа сонгоно уу?");
                return;
            }

            selectedBranch = (BranchModel)branchDataGrid.SelectedItem;
            EditBranchControl editControl = new EditBranchControl();
            editControl.selectedBranch = selectedBranch;
            externalContents.Content = editControl;
            editControl.UpdateDataGridEvent += EditControl_UpdateDataGridEvent1;
        }

        private void EditControl_UpdateDataGridEvent1(object sender, string e)
        {
            Task.Run(async () => await LoadDataGrid());
        }

        private void leaveAdminButton_Click(object sender, RoutedEventArgs e)
        {
            staffListText.Visibility = Visibility.Collapsed;
            staffDataGrid.Visibility = Visibility.Collapsed;
            staffEditButtons.Visibility = Visibility.Collapsed;
            branchListText.Visibility = Visibility.Collapsed;
            branchDataGrid.Visibility = Visibility.Collapsed;
            branchEditButtons.Visibility = Visibility.Collapsed;
            externalContents.Visibility = Visibility.Collapsed;
            leaveAdminButton.Visibility = Visibility.Collapsed;
            reloadListButton.Visibility = Visibility.Collapsed;

            BackControl.Content = new AuthenticationControl();
        }

        private void exportToExcel_Click(object sender, RoutedEventArgs e)
        {
            externalContents.Content = new ExportToExcelControl();
        }

        private async void reloadListButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataGrid();
        }
    }
}

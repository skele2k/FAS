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
        Thread refreshThread = null;
        public AdminControl()
        {
            InitializeComponent();

            //refreshThread = new Thread(new ThreadStart(AutoRefresher));
            //refreshThread.IsBackground = true;
            //refreshThread.Start();
        }
        //private void AutoRefresher()
        //{
        //    while (true)
        //    {
        //        if (refreshNoticer.refreshNow == true)
        //        {
        //            this.Dispatcher.Invoke(async () =>
        //            {
        //                Thread.Sleep(100);
        //                await LoadDataGrid();
        //                refreshNoticer.refreshNow = false;
        //            });
        //        }
        //    }
        //}
        private async Task LoadDataGrid()
        {
            try
            {
                var staffs = await ApiProcessor.LoadStaffs();
                staffDataGrid.ItemsSource = staffs;

                var branches = await ApiProcessor.LoadBranches();
                branchDataGrid.ItemsSource = branches;
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataGrid();
        }

        private void AddNewStaffButton_Click(object sender, RoutedEventArgs e)
        {
            externalContents.Content = new AddUserControl();
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
        }

        private void addNewBranchButton_Click(object sender, RoutedEventArgs e)
        {
            externalContents.Content = new AddBranchControl();
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

            try
            {
                refreshThread.Abort();
            }
            catch { }

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

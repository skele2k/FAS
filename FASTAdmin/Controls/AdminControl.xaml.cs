﻿using FASLib.Helpers;
using FASLib.Models;
using FASTAdmin.excelHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Dictionary<int, string> branchNameMapper = new Dictionary<int, string>();
        Dictionary<DisplayStaffModel, StaffModel> staffMapper = new Dictionary<DisplayStaffModel, StaffModel>();

        private static ObservableCollection<DisplayStaffModel> displayStaffs = new ObservableCollection<DisplayStaffModel>();
        public ObservableCollection<DisplayStaffModel> StaffCollection
        {
            get
            {
                return displayStaffs;
            }
            set
            {
                displayStaffs = value;
            }
        }

        public AdminControl()
        {
            InitializeComponent();
            MapBranchName();
        }
        private void MapBranchName()
        {
            var temp = Task.Run(async () => await ApiProcessor.LoadBranches());
            var branches = temp.Result;
            branchNameMapper.Clear();
            foreach(var branch in branches)
            {
                branchNameMapper[branch.id] = branch.name;
            }
        }
        private ObservableCollection<DisplayStaffModel> GenerateNewModels(List<StaffModel> staffs)
        {
            ObservableCollection<DisplayStaffModel> newStaffs = new ObservableCollection<DisplayStaffModel>();
            foreach(var staff in staffs)
            {
                DisplayStaffModel newStaff = new DisplayStaffModel();
                newStaff.firstName = staff.firstName;
                newStaff.lastName = staff.lastName;
                if (branchNameMapper.ContainsKey(staff.branch_id))
                {
                    newStaff.branchName = branchNameMapper[staff.branch_id];
                }
                else
                {
                    newStaff.branchName = "Устсан тасаг";
                }
                newStaffs.Add(newStaff);
                if (!staffMapper.ContainsKey(newStaff))
                {
                    staffMapper[newStaff] = staff;
                }
            }
            return newStaffs;
        }
        private async Task LoadDataGrid()
        {
            try
            {
                var staffs = await ApiProcessor.LoadStaffs();
                displayStaffs = GenerateNewModels(staffs);
                var branches = await ApiProcessor.LoadBranches();
                this.Dispatcher.Invoke(() => 
                { 
                    staffDataGrid.ItemsSource = StaffCollection;
                    branchDataGrid.ItemsSource = branches;
                });
            }
            catch(Exception e)
            {
                //MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
                MessageBox.Show(e.ToString());
            }
        }

        private async Task LoadStaffDataGrid()
        {
            try
            {
                var staffs = await ApiProcessor.LoadStaffs();
                displayStaffs = GenerateNewModels(staffs);
                this.Dispatcher.Invoke(() =>
                {
                    staffDataGrid.ItemsSource = StaffCollection;
                });
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private async Task LoadBranchDataGrid()
        {
            try
            {
                var branches = await ApiProcessor.LoadBranches();
                this.Dispatcher.Invoke(() =>
                {
                    branchDataGrid.ItemsSource = branches;
                });
            }
            catch(Exception e)
            {
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
            Task.Run(async () => await LoadStaffDataGrid());
        }

        private void deleteSelectedItemButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (staffDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Устгах ажилтнаа сонгоно уу?");
                return;
            }

            DisplayStaffModel selectedDisplayStaff = (DisplayStaffModel) staffDataGrid.SelectedItem;
            StaffModel selectedStaff = staffMapper[selectedDisplayStaff];

            DeleteUserControl deleteControl = new DeleteUserControl();
            deleteControl.selectedStaff = selectedStaff;
            externalContents.Content = deleteControl;
            deleteControl.UpdateDataGridEvent += DeleteControl_UpdateDataGridEvent;
        }

        private void DeleteControl_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadStaffDataGrid());
        }

        private void EditSelectedItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (staffDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Янзлах ажилтнаа сонгоно уу?");
                return;
            }

            DisplayStaffModel selectedDisplayStaff = (DisplayStaffModel)staffDataGrid.SelectedItem;
            StaffModel selectedStaff = staffMapper[selectedDisplayStaff];

            EditUserControl editControl = new EditUserControl();
            editControl.selectedStaff = selectedStaff;
            externalContents.Content = editControl;
            editControl.UpdateDataGridEvent += EditControl_UpdateDataGridEvent;
        }

        private void EditControl_UpdateDataGridEvent(object sender, string e)
        {
            Task.Run(async () => await LoadStaffDataGrid());
        }

        private void addNewBranchButton_Click(object sender, RoutedEventArgs e)
        {
            AddBranchControl addBranch = new AddBranchControl();
            externalContents.Content = addBranch;
            addBranch.UpdateDataGridEvent += AddBranch_UpdateDataGridEvent;
        }

        private void AddBranch_UpdateDataGridEvent(object sender, string e)
        {
            MapBranchName();
            Task.Run(async () => await LoadBranchDataGrid());
        }

        private void deleteSelectedBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (branchDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Устгах тасгаа сонгоно уу?");
                return;
            }

            BranchModel selectedBranch = (BranchModel) branchDataGrid.SelectedItem;
            DeleteBranchControl deleteControl = new DeleteBranchControl();
            deleteControl.selectedBranch = selectedBranch;
            externalContents.Content = deleteControl;
            deleteControl.UpdateDataGridEvent += DeleteControl_UpdateDataGridEvent1;
        }

        private void DeleteControl_UpdateDataGridEvent1(object sender, string e)
        {
            MapBranchName();
            Task.Run(async () => await LoadBranchDataGrid());
        }

        private void EditSelectedBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (branchDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Янзлах тасгаа сонгоно уу?");
                return;
            }

            BranchModel selectedBranch = (BranchModel)branchDataGrid.SelectedItem;
            EditBranchControl editControl = new EditBranchControl();
            editControl.selectedBranch = selectedBranch;
            externalContents.Content = editControl;
            editControl.UpdateDataGridEvent += EditControl_UpdateDataGridEvent1;
        }

        private void EditControl_UpdateDataGridEvent1(object sender, string e)
        {
            MapBranchName();
            Task.Run(async () => await LoadBranchDataGrid());
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

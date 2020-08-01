using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using FASLib.DataAccess;
using FASLib.Helpers;
using FASLib.Models;


namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for EditBranchControl.xaml
    /// </summary>
    public partial class EditBranchControl : UserControl
    {
        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        public EditBranchControl()
        {
            InitializeComponent();
            WireUpBranchesDropdown();
        }

        private void WireUpBranchesDropdown()
        {
            branchSelectDropDown.ItemsSource = branches;
            branchSelectDropDown.DisplayMemberPath = "name";
            branchSelectDropDown.SelectedValuePath = "id";
        }

        private async Task InitializeBranchList()
        {
            var branchesList = await ApiProcessor.LoadBranches();
            branchesList.ForEach(x => branches.Add(x));
        }

        private (bool isValid, BranchModel model) ValidateForm()
        {
            bool isValid = true;
            BranchModel model = new BranchModel();
            try
            {
                model.id = (int) branchSelectDropDown.SelectedValue;
                model.name = newBranchname.Text;
            }
            catch
            {
                isValid = false;
            }
            return (isValid, model);
        }
        private void ClearForm()
        {
            newBranchname.Text = "";
            branchSelectDropDown.SelectedItem = null;
        }
        private async void AddBranchToDatabase()
        {
            var form = ValidateForm();
            if (form.isValid == false)
            {
                MessageBox.Show("Та тохирох өгөгдөл оруулна уу?");
                return;
            }

            var t = await ApiProcessor.EditBranchByID(form.model);

            if (t == "success")
            {
                MessageBox.Show("Амжилттай шинэчиллээ.");
            }
            else
            {
                MessageBox.Show("Алдаа гарлаа. Та дахин оролдоно уу?");
            }
            ClearForm();
            branches.Clear();
            await InitializeBranchList();
            WireUpBranchesDropdown();
        }
        private void submitNewBranch_Click(object sender, RoutedEventArgs e)
        {
            if(branchSelectDropDown.SelectedItem == null)
            {
                MessageBox.Show("Янзлах тасгаа сонгоно уу?");
                return;
            }
            if (newBranchname.Text == "")
            {
                MessageBox.Show("Шинэ нэр өгнө үү?");
                return;
            }

            AddBranchToDatabase();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            editBranchTextBlock.Visibility = Visibility.Collapsed;
            selectBranchStackPanel.Visibility = Visibility.Collapsed;
            newNameStackPanel.Visibility = Visibility.Collapsed;
            submitNewBranch.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchList();
        }
    }
}

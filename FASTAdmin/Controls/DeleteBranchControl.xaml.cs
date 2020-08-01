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
    /// Interaction logic for DeleteBranchControl.xaml
    /// </summary>
    public partial class DeleteBranchControl : UserControl
    {
        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        public DeleteBranchControl()
        {
            InitializeComponent();
            WireUpBranchDropdown();
        }
        private async Task InitializeBranchesToList()
        {
            var branchesList = await ApiProcessor.LoadBranches();
            branchesList.ForEach(x => branches.Add(x));
        }
        private void WireUpBranchDropdown()
        {
            branchSelectDropdown.ItemsSource = branches;
            branchSelectDropdown.DisplayMemberPath = "name";
            branchSelectDropdown.SelectedValuePath = "id";
        }

        private async void deleteBranch_Click(object sender, RoutedEventArgs e)
        {
            if (branchSelectDropdown.SelectedItem == null)
            {
                MessageBox.Show("Устгах тасгаа сонгоно уу?");
                return;
            }

            BranchModel model = (BranchModel) branchSelectDropdown.SelectedItem;
            var t = await ApiProcessor.DeleteBranchByID(model.id);
            if (t == "success")
            {
                MessageBox.Show("Амжилттай устгалаа.");
            }
            else
            {
                MessageBox.Show("Устгалт амжилтгүй.");
            }
            ResetForm();
            branches.Clear();
            await InitializeBranchesToList();
            WireUpBranchDropdown();
        }

        private void ResetForm()
        {
            branchSelectDropdown.SelectedItem = null;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            deleteBranchTextBlock.Visibility = Visibility.Collapsed;
            selectBranchStackPanel.Visibility = Visibility.Collapsed;
            deleteStaff.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();

        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchesToList();
        }
    }
}

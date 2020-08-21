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
        public BranchModel selectedBranch { get; set; }
        public DeleteBranchControl()
        {
            InitializeComponent();
            WireUpBranchDropdown();
        }
        private async Task InitializeBranchesToList()
        {
            try
            {
                var branchesList = await ApiProcessor.LoadBranches();
                branchesList.ForEach(x => branches.Add(x));
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
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

            try
            {
                BranchModel model = (BranchModel)branchSelectDropdown.SelectedItem;
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
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
        }

        private void ResetForm()
        {
            deleteBranchTextBlock.Visibility = Visibility.Collapsed;
            selectBranchStackPanel.Visibility = Visibility.Collapsed;
            deleteStaff.Visibility = Visibility.Collapsed;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchesToList();

            int left = 0;
            int right = branches.Count() - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (branches[mid].id == selectedBranch.id)
                {
                    branchSelectDropdown.SelectedItem = branches[mid];
                    break;
                }
                else if (branches[mid].id > selectedBranch.id)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
        }
    }
}

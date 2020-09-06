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
        public event EventHandler<string> UpdateDataGridEvent;

        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        public BranchModel selectedBranch { get; set; }
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
        private void AddBranchToDatabase()
        {
            var form = ValidateForm();
            if (form.isValid == false)
            {
                MessageBox.Show("Та тохирох өгөгдөл оруулна уу?");
                return;
            }
            try
            {
                var t = Task.Run(async () => await ApiProcessor.EditBranchByID(form.model));
                var result = t.Result;
                if (result != "success")
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }

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
            ResetForm();

            UpdateDataGridEvent?.Invoke(this, "");
        }
        private void ResetForm()
        {
            editBranchTextBlock.Visibility = Visibility.Collapsed;
            selectBranchStackPanel.Visibility = Visibility.Collapsed;
            newNameStackPanel.Visibility = Visibility.Collapsed;
            submitNewBranch.Visibility = Visibility.Collapsed;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeBranchList();

            int left = 0;
            int right = branches.Count() - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (branches[mid].id == selectedBranch.id)
                {
                    branchSelectDropDown.SelectedItem = branches[mid];
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

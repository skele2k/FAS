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
            InitializeBranchList();
            WireUpBranchesDropdown();
        }

        private void WireUpBranchesDropdown()
        {
            branchSelectDropDown.ItemsSource = branches;
            branchSelectDropDown.DisplayMemberPath = "name";
            branchSelectDropDown.SelectedValuePath = "id";
        }

        private void InitializeBranchList()
        {
            string sql = "SELECT * FROM branch";
            var branchesList = SqliteDataAccess.LoadData<BranchModel>(sql, new Dictionary<string, object>());
            branchesList.ForEach(x => branches.Add(x));
        }

        private (bool isValid, BranchModel model) ValidateForm()
        {
            bool isValid = true;
            BranchModel model = new BranchModel();
            try
            {
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
        private void AddBranchToDatabase()
        {
            var form = ValidateForm();
            if (form.isValid == false)
            {
                MessageBox.Show("Та тохирох өгөгдөл оруулна уу?");
                return;
            }

            string sql = "UPDATE branch SET name = @name WHERE id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@id", (int)branchSelectDropDown.SelectedValue },
                {"@name", form.model.name }
            };
            SqliteDataAccess.SaveData(sql, parameters);

            MessageBox.Show("Амжилттай шинэчиллээ.");
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
            ClearForm();
            branches.Clear();
            InitializeBranchList();
            WireUpBranchesDropdown();
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
    }
}

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
    /// Interaction logic for DeleteBranchControl.xaml
    /// </summary>
    public partial class DeleteBranchControl : UserControl
    {
        ObservableCollection<BranchModel> branches = new ObservableCollection<BranchModel>();
        public DeleteBranchControl()
        {
            InitializeComponent();
            InitializeBranchesToList();
            WireUpBranchDropdown();
        }
        private void InitializeBranchesToList()
        {
            string sql = "SELECT * FROM branch";
            var branchList = SqliteDataAccess.LoadData<BranchModel>(sql, new Dictionary<string, object>());
            branchList.ForEach(x => branches.Add(x));
        }
        private void WireUpBranchDropdown()
        {
            branchSelectDropdown.ItemsSource = branches;
            branchSelectDropdown.DisplayMemberPath = "name";
            branchSelectDropdown.SelectedValuePath = "id";
        }

        private void deleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (branchSelectDropdown.SelectedItem == null)
            {
                MessageBox.Show("Устгах тасгаа сонгоно уу?");
                return;
            }

            BranchModel model = (BranchModel) branchSelectDropdown.SelectedItem;
            string sql = "DELETE FROM branch WHERE name = @name AND id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@id", model.id },
                {"@name", model.name}
            };

            SqliteDataAccess.SaveData(sql, parameters);
            ResetForm();
            MessageBox.Show("Амжилттай устгалаа.");
            branches.Clear();
            InitializeBranchesToList();
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
    }
}

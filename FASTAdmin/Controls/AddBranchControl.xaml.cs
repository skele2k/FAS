using FASLib.Models;
using FASLib.DataAccess;
using System;
using System.Collections.Generic;
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
using FASLib.Helpers;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for AddBranchControl.xaml
    /// </summary>
    public partial class AddBranchControl : UserControl
    {
        public event EventHandler<string> UpdateDataGridEvent;
        public AddBranchControl()
        {
            InitializeComponent();
        }
        private async void AddBranchToDatabase()
        {
            var form = Validate();
            
            if (form.isValid == false)
            {
                MessageBox.Show("Форм бөглөхөд алдаа гарлаа. Та формоо зөв бөглөнө үү?");
                return;
            }
            try
            {
                var t = await ApiProcessor.SaveBranch(form.model);
                if (t != "success")
                {
                    MessageBox.Show("Алдаа гарлаа. Та дахин оролдоно уу?");
                }
            }
            catch
            {
                MessageBox.Show("Сүлжээний алдаа. Сүлжээнд холбогдсон эсэхээ шалгана уу?");
            }
        }

        private void ResetForm()
        {
            addBranchTextBlock.Visibility = Visibility.Collapsed;
            addBranchNameTextBox.Visibility = Visibility.Collapsed;
            addBranchButton.Visibility = Visibility.Collapsed;
        }

        private (bool isValid, BranchModel model) Validate()
        {
            bool isValid = true;
            BranchModel model = new BranchModel();
            try
            {
                model.name = BranchNameTextBox.Text;
            }
            catch
            {
                isValid = false;
            }
            return (isValid, model);
        }

        private void addBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (BranchNameTextBox.Text == "")
            {
                MessageBox.Show("Та шинэ тасгийн нэрээ өгнө үү?");
                return;
            }
            AddBranchToDatabase();
            ResetForm();

            UpdateDataGridEvent?.Invoke(this, "");
        }
    }
}

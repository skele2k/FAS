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
using System.Windows.Shapes;

namespace FASTAdmin
{
    /// <summary>
    /// Interaction logic for DeleteConfirmation.xaml
    /// </summary>
    public partial class DeleteConfirmation : Window
    {
        bool canDelete = false;
        public bool CanDelete
        {
            get
            {
                return canDelete;
            }
        }
        public DeleteConfirmation()
        {
            InitializeComponent();
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            canDelete = true;
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void declineButton_Click(object sender, RoutedEventArgs e)
        {
            canDelete = false;
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }
    }
}

using FASTAdmin.excelHelper;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for ExportToExcelControl.xaml
    /// </summary>
    public partial class ExportToExcelControl : UserControl
    {
        private string thePath = "";
        public event EventHandler<string> fileLocationIsSet;
        public ExportToExcelControl()
        {
            InitializeComponent();
            InitializeUserControl();
            this.fileLocationIsSet += ExportToExcelControl_fileLocationIsSet;
        }

        private void ExportToExcelControl_fileLocationIsSet(object sender, string e)
        {
            fileLocationTextblock.Text = thePath;
        }

        private void InitializeUserControl()
        {
            startDataPicker.SelectedDate = DateTime.UtcNow;
            endDataPicker.SelectedDate = DateTime.UtcNow;

            CalendarsStackPanel.Visibility = Visibility.Hidden;
        }
        private void exportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (startDataPicker.SelectedDate.ToString() == "" || endDataPicker.SelectedDate.ToString() == "")
            {
                MessageBox.Show("Эхлэх эсвэл дуусах өдөр сонгогдоогүй байна.");
                return;
            }
            if (fileLocationTextblock.Text == "")
            {
                MessageBox.Show("Хуулах газраа сонгоно уу?");
                return;
            }
            DateTime startDate = (DateTime) startDataPicker.SelectedDate;
            DateTime endDate = (DateTime) endDataPicker.SelectedDate;

            if (periodSelectCheckbox.IsChecked == true)
            {
                if (endDate.CompareTo(startDate) >= 0)
                {
                    xlHelper.PeriodDataExporter(startDate, endDate, thePath);
                }
                else
                {
                    MessageBox.Show("Эхлэх хугацаа дуусах хугацаанаас их байж болохгүй.");
                }
            }
            else
            {
                xlHelper.AllDataExporter(thePath);
            }
        }

        private void periodSelectCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CalendarsStackPanel.Visibility = Visibility.Visible;
        }

        private void periodSelectCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CalendarsStackPanel.Visibility = Visibility.Hidden;
        }

        private void fileLocationButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                var checker = result.ToString();
                if (checker == "OK")
                {
                    thePath = dialog.SelectedPath;
                    fileLocationIsSet?.Invoke(this, "");
                }
            }
        }
    }
}

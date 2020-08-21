using FASTAdmin.excelHelper;
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

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for ExportToExcelControl.xaml
    /// </summary>
    public partial class ExportToExcelControl : UserControl
    {
        public ExportToExcelControl()
        {
            InitializeComponent();
        }

        private void exportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (startDataPicker.SelectedDate.ToString() == "" || endDataPicker.SelectedDate.ToString() == "")
            {
                MessageBox.Show("Эхлэх эсвэл дуусах өдөр сонгогдоогүй байна.");
                return;
            }
            DateTime startDate = (DateTime) startDataPicker.SelectedDate;
            DateTime endDate = (DateTime) endDataPicker.SelectedDate;

            if (endDate.CompareTo(startDate) > 0)
            {
                xlHelper.PeriodDataExporter(startDate, endDate);
            }
            else
            {
                MessageBox.Show("Эхлэх хугацаа дуусах хугацаанаас их байж болохгүй.");
            }
        }
    }
}

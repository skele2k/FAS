using FASLib.DataAccess;
using FASLib.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace FASTAdmin.excelHelper
{
    public static class xlHelper
    {
        public async static void AllDataExporter()
        {

            string untilUser = System.Environment.GetEnvironmentVariable("USERPROFILE");
            System.IO.Directory.CreateDirectory(untilUser + @"\Desktop\FASxlOutput");
            FileInfo newFile = new FileInfo(untilUser + @"\Desktop\FASxlOutput\allTimeAttendance.xlsx");

            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(untilUser + @"\Desktop\FASxlOutput\allTimeAttendance.xlsx");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // adding new worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Бүх ирц");

                // adding headers
                worksheet.Cells[1, 1].Value = "Нэр";
                worksheet.Cells[1, 2].Value = "Тасаг";
                worksheet.Cells[1, 3].Value = "Өдрийн хоол";
                worksheet.Cells[1, 4].Value = "Ирсэн цаг";
                worksheet.Cells[1, 5].Value = "Явсан цаг";
                worksheet.Cells[1, 6].Value = "Ажилласан цаг";
                worksheet.Cells[1, 7].Value = "Ажил дээрээ байгаа эсэх";
                worksheet.Cells[1, 8].Value = "Өдөр";
                
                var attendanceList = await ApiProcessor.LoadAttendanceSheet();
                int size = attendanceList.Count();

                for (int i = 0; i < size; i++)
                {
                    worksheet.Cells[$"A{ i + 2 }"].Value = attendanceList[i].staff_id;
                    worksheet.Cells[$"B{ i + 2 }"].Value = attendanceList[i].branch_id;
                    worksheet.Cells[$"C{ i + 2 }"].Value = attendanceList[i].arriveTime;
                    worksheet.Cells[$"D{ i + 2 }"].Value = attendanceList[i].leaveTime;
                    worksheet.Cells[$"E{ i + 2 }"].Value = attendanceList[i].officeHours;
                    worksheet.Cells[$"F{ i + 2 }"].Value = attendanceList[i].atOffice;
                    worksheet.Cells[$"G{ i + 2 }"].Value = attendanceList[i].date;
                }

                package.Save();
            }
            MessageBox.Show("Хөрвүүлж дууслаа!");
        }
    }
}

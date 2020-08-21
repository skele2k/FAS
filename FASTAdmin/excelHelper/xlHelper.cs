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
        public async static void PeriodDataExporter(DateTime startDate, DateTime endDate)
        {
            string untilUser = System.Environment.GetEnvironmentVariable("USERPROFILE");
            System.IO.Directory.CreateDirectory(untilUser + @"\Desktop\FASxlOutput");

            string startDateStr = startDate.ToString("MM_dd_yyyy");
            string endDateStr = endDate.ToString("MM_dd_yyyy");

            FileInfo newFile = new FileInfo(untilUser + $"\\Desktop\\FASxlOutput\\{ startDateStr }-{ endDateStr }.xlsx");
        
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(untilUser + $"\\Desktop\\FASxlOutput\\{ startDateStr }-{ endDateStr }.xlsx");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{startDateStr}-{endDateStr}");

                worksheet.Cells[1, 1].Value = "Нэр";
                worksheet.Cells[1, 2].Value = "Тасаг";
                worksheet.Cells[1, 3].Value = "Ирсэн цаг";
                worksheet.Cells[1, 4].Value = "Явсан цаг";
                worksheet.Cells[1, 5].Value = "Ажилласан цаг";
                worksheet.Cells[1, 6].Value = "Ажил дээрээ байгаа эсэх";
                worksheet.Cells[1, 7].Value = "Өдөр";

                var attendanceList = await ApiProcessor.LoadAttendanceSheet();
                int size = attendanceList.Count();

                var branchList = await ApiProcessor.LoadBranches();
                var staffList = await ApiProcessor.LoadStaffs();

                Dictionary<int, string> staffNameMapper = new Dictionary<int, string>();
                Dictionary<int, string> branchNameMapper = new Dictionary<int, string>();

                int staffSize = staffList.Count();
                int branchSize = branchList.Count();

                for (int i = 0; i < staffSize; i++)
                {
                    staffNameMapper[staffList[i].id] = staffList[i].fullName;
                }

                for (int i = 0; i < staffSize; i++)
                {
                    branchNameMapper[branchList[i].id] = branchList[i].name;
                }

                for (int i = 0; i < size; i++)
                {
                    DateTime currentDate = DateTime.Parse(attendanceList[i].date);
                    string currentDateStr = currentDate.ToString();
                    if (currentDate.CompareTo(startDate) < 0)
                    {
                        continue;
                    }
                    if (currentDate.CompareTo(endDate) > 0)
                    {
                        break;
                    }


                    if (staffNameMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        worksheet.Cells[$"A{ i + 2 }"].Value = staffNameMapper[attendanceList[i].staff_id];
                    }
                    else
                    {
                        worksheet.Cells[$"A{ i + 2 }"].Value = attendanceList[i].staff_id;
                    }

                    if (branchNameMapper.ContainsKey(attendanceList[i].branch_id))
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = branchNameMapper[attendanceList[i].branch_id];
                    }
                    else
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = attendanceList[i].branch_id;
                    }

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
                worksheet.Cells[1, 3].Value = "Ирсэн цаг";
                worksheet.Cells[1, 4].Value = "Явсан цаг";
                worksheet.Cells[1, 5].Value = "Ажилласан цаг";
                worksheet.Cells[1, 6].Value = "Ажил дээрээ байгаа эсэх";
                worksheet.Cells[1, 7].Value = "Өдөр";
                
                var attendanceList = await ApiProcessor.LoadAttendanceSheet();
                int size = attendanceList.Count();

                var branchList = await ApiProcessor.LoadBranches();
                var staffList = await ApiProcessor.LoadStaffs();

                Dictionary<int, string> staffNameMapper = new Dictionary<int, string>();
                Dictionary<int, string> branchNameMapper = new Dictionary<int, string>();

                int staffSize = staffList.Count();
                int branchSize = branchList.Count();

                for (int i = 0; i < staffSize; i++)
                {
                    staffNameMapper[staffList[i].id] = staffList[i].fullName;
                }

                for (int i = 0; i < staffSize; i++)
                {
                    branchNameMapper[branchList[i].id] = branchList[i].name;
                }

                for (int i = 0; i < size; i++)
                {
                    if (staffNameMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        worksheet.Cells[$"A{ i + 2 }"].Value = staffNameMapper[attendanceList[i].staff_id];
                    }
                    else
                    {
                        worksheet.Cells[$"A{ i + 2 }"].Value = attendanceList[i].staff_id;
                    }

                    if (branchNameMapper.ContainsKey(attendanceList[i].branch_id))
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = branchNameMapper[attendanceList[i].branch_id];
                    }
                    else
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = attendanceList[i].branch_id;
                    }

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

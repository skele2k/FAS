using FASLib.DataAccess;
using FASLib.Helpers;
using FASLib.Models;
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
    class comparer : IComparer<StaffModel>
    {
        public int Compare(StaffModel x, StaffModel y)
        {
            if (x.branch_id == y.branch_id)
            {
                return x.firstName.CompareTo(y.firstName);
            }
            else
            {
                bool first = x.branch_id.CompareTo(y.branch_id) != 0;
                bool second = x.firstName.CompareTo(y.firstName) != 0;
                return Convert.ToInt32(first && second);
            }
        }
    }
    public static class xlHelper
    {
        public async static void PeriodDataExporter(DateTime startDate, DateTime endDate, string path)
        {
            string startDateStr = startDate.ToString("MM_dd_yyyy");
            string endDateStr = endDate.ToString("MM_dd_yyyy");

            string filePath = path + $"\\{ startDateStr }-{ endDateStr }.xlsx";


            FileInfo newFile = new FileInfo(filePath);

            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();
                }
                catch
                {
                    MessageBox.Show("Нээлттэй эксэл файлаа хаана уу?");
                    return;
                }
                newFile = new FileInfo(filePath);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add($"{startDateStr}-{endDateStr}");
                
                worksheet.Cells[1, 1].Value = "Өдөр";
                worksheet.Cells[1, 2].Value = "Тасаг";
                worksheet.Cells[1, 3].Value = "Нэр";
                worksheet.Cells[1, 4].Value = "Ирсэн цаг";
                worksheet.Cells[1, 5].Value = "Явсан цаг";
                worksheet.Cells[1, 6].Value = "Ажилласан цаг";
                worksheet.Cells[1, 7].Value = "Ажил дээрээ байгаа эсэх";
                

                var attendanceList = await ApiProcessor.LoadAttendanceSheet();
                int size = attendanceList.Count();

                var branchList = await ApiProcessor.LoadBranches();
                var staffList = await ApiProcessor.LoadStaffs();

                Dictionary<int, string> staffNameMapper = new Dictionary<int, string>();
                Dictionary<int, string> branchNameMapper = new Dictionary<int, string>();
                Dictionary<int, double> staffWorkHourMapper = new Dictionary<int, double>();

                int staffSize = staffList.Count();
                int branchSize = branchList.Count();

                for (int i = 0; i < staffSize; i++)
                {
                    staffNameMapper[staffList[i].id] = staffList[i].fullName;
                }

                for (int i = 0; i < branchSize; i++)
                {
                    branchNameMapper[branchList[i].id] = branchList[i].name;
                }
                int left = 0;
                int right = size - 1;
                int startIndex = 0;
                while (left <= right)
                {
                    int mid = left + (right - left) / 2;
                    var midDate = DateTime.Parse(attendanceList[mid].date);
                    if (midDate.CompareTo(startDate) == 0)
                    {
                        startIndex = mid;
                        break;
                    }
                    if (midDate.CompareTo(startDate) > 0)
                    {
                        right = mid - 1;
                    }
                    else if (midDate.CompareTo(startDate) < 0)
                    {
                        left = mid + 1;
                        startIndex = left;
                    }
                }
                for (int i = startIndex; i > 0; i--)
                {
                    if (startIndex == size)
                    {
                        break;
                    }
                    var currentDate = DateTime.Parse(attendanceList[i].date);
                    var prevDate = DateTime.Parse(attendanceList[i - 1].date);
                    if (currentDate.CompareTo(prevDate) > 0)
                    {
                        startIndex = i;
                        break;
                    }

                    if (i == 1)
                    {
                        startIndex = 0;
                    }
                }
                List<AttendanceModel> outList = new List<AttendanceModel>();

                for (int i = startIndex; i < size; i++)
                {
                    outList.Add(attendanceList[i]);
                    DateTime currentDate = DateTime.Parse(attendanceList[i].date);
                    if (currentDate.CompareTo(endDate) > 0)
                    {
                        break;
                    }
                }

                int k = 0;
                for (int i = startIndex; i < size; i++)
                {
                    DateTime currentDate = DateTime.Parse(attendanceList[i].date);
                    string currentDateStr = currentDate.ToString();

                    if (currentDate.CompareTo(endDate) > 0)
                    {
                        break;
                    }


                    worksheet.Cells[$"A{ k + 2 }"].Value = attendanceList[i].date;
          
                    if (branchNameMapper.ContainsKey(attendanceList[i].branch_id))
                    {
                        worksheet.Cells[$"B{ k + 2 }"].Value = branchNameMapper[attendanceList[i].branch_id];
                    }
                    else
                    {
                        worksheet.Cells[$"B{ k + 2 }"].Value = attendanceList[i].branch_id;
                    }

                    if (staffNameMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        worksheet.Cells[$"C{ k + 2 }"].Value = staffNameMapper[attendanceList[i].staff_id];
                    }
                    else
                    {
                        worksheet.Cells[$"C{ k + 2 }"].Value = attendanceList[i].staff_id;
                    }


                    worksheet.Cells[$"D{ k + 2 }"].Value = attendanceList[i].arriveTime;
                    worksheet.Cells[$"E{ k + 2 }"].Value = attendanceList[i].leaveTime;
                    worksheet.Cells[$"F{ k + 2 }"].Value = attendanceList[i].officeHours;
                    worksheet.Cells[$"G{ k + 2 }"].Value = attendanceList[i].atOffice;

                    if (staffWorkHourMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        staffWorkHourMapper[attendanceList[i].staff_id] += attendanceList[i].officeHours;
                    }
                    else
                    {
                        staffWorkHourMapper[attendanceList[i].staff_id] = attendanceList[i].officeHours;
                    }

                    k++;
                }

                //Analysis part
                ExcelWorksheet analyze = package.Workbook.Worksheets.Add($"Тайлан");
                analyze.Cells[1, 1].Value = "Тасаг";
                analyze.Cells[1, 2].Value = "Нэр";
                analyze.Cells[1, 3].Value = "Сонгосон хугацаанд ажилласан цаг";

                comparer cmp = new comparer();
                List<StaffModel> display = staffList.OrderBy(s => branchNameMapper[s.branch_id]).ThenBy(s => s.firstName).ToList();

                for (int i = 0; i < staffSize; i++)
                {
                    analyze.Cells[$"A{ i + 2 }"].Value = branchNameMapper[display[i].branch_id];
                    analyze.Cells[$"B{ i + 2 }"].Value = staffNameMapper[display[i].id];
                    analyze.Cells[$"C{ i + 2 }"].Value = staffWorkHourMapper[display[i].id];
                }

                package.Save();
                MessageBox.Show("Хөрвүүлж дууслаа!");
                OpenFile(filePath);
            }
        }
        private static void OpenFile(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
        public async static void AllDataExporter(string path)
        {
            string filePath = path + @"\allTimeAttendance.xlsx";
            FileInfo newFile = new FileInfo(filePath);
            
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();
                }
                catch
                {
                    MessageBox.Show("Нээлттэй эксэл файлаа хаана уу?");
                    return;
                }
                newFile = new FileInfo(filePath);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                // adding new worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Бүх ирц");

                // adding headers
                worksheet.Cells[1, 1].Value = "Өдөр";
                worksheet.Cells[1, 2].Value = "Тасаг";
                worksheet.Cells[1, 3].Value = "Нэр";
                worksheet.Cells[1, 4].Value = "Ирсэн цаг";
                worksheet.Cells[1, 5].Value = "Явсан цаг";
                worksheet.Cells[1, 6].Value = "Ажилласан цаг";
                worksheet.Cells[1, 7].Value = "Ажил дээрээ байгаа эсэх";
                
                
                var attendanceList = await ApiProcessor.LoadAttendanceSheet();
                int size = attendanceList.Count();

                var branchList = await ApiProcessor.LoadBranches();
                var staffList = await ApiProcessor.LoadStaffs();

                Dictionary<int, string> staffNameMapper = new Dictionary<int, string>();
                Dictionary<int, string> branchNameMapper = new Dictionary<int, string>();
                Dictionary<int, double> staffWorkHourMapper = new Dictionary<int, double>();

                int staffSize = staffList.Count();
                int branchSize = branchList.Count();

                for (int i = 0; i < staffSize; i++)
                {
                    staffNameMapper[staffList[i].id] = staffList[i].fullName;
                }

                for (int i = 0; i < branchSize; i++)
                {
                    branchNameMapper[branchList[i].id] = branchList[i].name;
                }

                for (int i = 0; i < size; i++)
                {
                    worksheet.Cells[$"A{ i + 2 }"].Value = attendanceList[i].date;

                    if (branchNameMapper.ContainsKey(attendanceList[i].branch_id))
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = branchNameMapper[attendanceList[i].branch_id];
                    }
                    else
                    {
                        worksheet.Cells[$"B{ i + 2 }"].Value = attendanceList[i].branch_id;
                    }

                    if (staffNameMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        worksheet.Cells[$"C{ i + 2 }"].Value = staffNameMapper[attendanceList[i].staff_id];
                    }
                    else
                    {
                        worksheet.Cells[$"C{ i + 2 }"].Value = attendanceList[i].staff_id;
                    }


                    worksheet.Cells[$"D{ i + 2 }"].Value = attendanceList[i].arriveTime;
                    worksheet.Cells[$"E{ i + 2 }"].Value = attendanceList[i].leaveTime;
                    worksheet.Cells[$"F{ i + 2 }"].Value = attendanceList[i].officeHours;
                    worksheet.Cells[$"G{ i + 2 }"].Value = attendanceList[i].atOffice;

                    if (staffWorkHourMapper.ContainsKey(attendanceList[i].staff_id))
                    {
                        staffWorkHourMapper[attendanceList[i].staff_id] += attendanceList[i].officeHours;
                    }
                    else
                    {
                        staffWorkHourMapper[attendanceList[i].staff_id] = attendanceList[i].officeHours;
                    }
                }
                //Analysis part
                ExcelWorksheet analyze = package.Workbook.Worksheets.Add($"Тайлан");
                analyze.Cells[1, 1].Value = "Тасаг";
                analyze.Cells[1, 2].Value = "Нэр";
                analyze.Cells[1, 3].Value = "Сонгосон хугацаанд ажилласан цаг";

                comparer cmp = new comparer();
                List<StaffModel> display = staffList.OrderBy(s => branchNameMapper[s.branch_id]).ThenBy(s => s.firstName).ToList();

                for (int i = 0; i < staffSize; i++)
                {
                    analyze.Cells[$"A{ i + 2 }"].Value = branchNameMapper[display[i].branch_id];
                    analyze.Cells[$"B{ i + 2 }"].Value = staffNameMapper[display[i].id];
                    analyze.Cells[$"C{ i + 2 }"].Value = staffWorkHourMapper[display[i].id];
                }

                package.Save();
                MessageBox.Show("Хөрвүүлж дууслаа!");
                OpenFile(filePath);
            }
        }
    }
}

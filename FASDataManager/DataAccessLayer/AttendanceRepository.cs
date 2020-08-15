using FASLib.DataAccess;
using FASLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public class AttendanceRepository : IAttendanceRepository
    {
        public List<AttendanceModel> GetAttendance()
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM attendance";

            var output = SqliteDataAccess.LoadData<AttendanceModel>(sql, new Dictionary<string, object>());
            return output;
        }
        public List<AttendanceModel> GetAttendanceOfLastXDays(int X)
        {
            throw new NotImplementedException();
        }
        public bool AddStaffToAttendanceSheet(AttendanceModel theAttendance)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }
            string sql = "INSERT INTO attendance(staff_id, branch_id, date) VALUES(@staff_id, @branch_id, @date)";
            bool output = true;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@staff_id", theAttendance.staff_id },
                    {"@branch_id", theAttendance.branch_id },
                    {"@date", theAttendance.date },
                };
                SqliteDataAccess.SaveData(sql, parameters);
            }
            catch
            {
                output = false;
            }
            return output;
        }
        public bool EditAttendanceSheet(AttendanceModel theAttendance)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }
            bool output = true;
            if (theAttendance.arriveTime == null && theAttendance.leaveTime == null)
            {
                string sql = "UPDATE attendance SET atOffice = @atOffice WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";

                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"@atOffice", theAttendance.atOffice },
                        {"@staff_id", theAttendance.staff_id },
                        {"@branch_id", theAttendance.branch_id },
                        {"@date", theAttendance.date }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                }
                catch
                {
                    output = false;
                }
            }
            else if (theAttendance.arriveTime != null && theAttendance.leaveTime == null)
            {
                string sql = "UPDATE attendance SET arriveTime = @arriveTime, atOffice = @atOffice WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"@arriveTime", theAttendance.arriveTime },
                        {"@atOffice", theAttendance.atOffice },
                        {"@staff_id", theAttendance.staff_id },
                        {"@branch_id", theAttendance.branch_id },
                        {"@date", theAttendance.date }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                }
                catch
                {
                    output = false;
                }
            }
            else
            {
                string sql = "UPDATE attendance SET leaveTime = @leaveTime, atOffice = @atOffice, officeHours = @officeHours WHERE staff_id = @staff_id AND branch_id = @branch_id AND date = @date";
                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"@leaveTime", theAttendance.leaveTime },
                        {"@atOffice", theAttendance.atOffice },
                        {"@officeHours", theAttendance.officeHours },
                        {"@staff_id", theAttendance.staff_id },
                        {"@branch_id", theAttendance.branch_id },
                        {"@date", theAttendance.date }
                    };
                    SqliteDataAccess.SaveData(sql, parameters);
                }
                catch
                {
                    output = false;
                }
            }
            return output;
        }
    }
}
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
            string sql = "INSERT INTO attendance(staff_id, branch_id, date, hasLunch) VALUES(@staff_id, @branch_id, @date, @hasLunch)";
            bool output = true;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@staff_id", theAttendance.staff_id },
                    {"@branch_id", theAttendance.branch_id },
                    {"@date", theAttendance.date },
                    {"@hasLunch", theAttendance.hasLunch }
                };
                SqliteDataAccess.SaveData(sql, parameters);
            }
            catch
            {
                output = false;
            }
            return output;
        }
    }
}
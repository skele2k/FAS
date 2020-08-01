using FASLib.Models;
using FASLib.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Dapper;
using System.Web.Configuration;

namespace FASDataManager.DataAccessLayer
{
    public class StaffRepository : IStaffRepository
    {
        // Todo: implement exceptions
        public List<StaffModel> GetStaffs()
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM staff";

            var output = SqliteDataAccess.LoadData<StaffModel>(sql, new Dictionary<string, object>());
            return output;
        }

        public StaffModel GetSingleStaff(int staffId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM staff WHERE id = @staffId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@staffId", staffId }
            };

            var output = SqliteDataAccess.LoadData<StaffModel>(sql, parameters).FirstOrDefault();
            return output;
            //try
            //{
            //    using (var cnn = SqliteBaseRepository.SimpleDbConnection())
            //    {
            //        cnn.Open();
            //        var returnStaff = cnn.Query<StaffModel>(
            //                @"SELECT * FROM staff WHERE id = @staffId", new { staffId }).FirstOrDefault();
            //        return returnStaff;
            //    }
            //}
            //catch
            //{
            //    return null;
            //}
        }
        public bool InsertStaff(StaffModel newStaff)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                SqliteBaseRepository.CreateDatabase();
            }
            string sql = "insert into staff(branch_id, firstname, lastname, fingerPrint ,haslunch) values(@branch_id, @firstname, @lastname, @fingerPrint, @haslunch)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", newStaff.branch_id },
                {"@firstname", newStaff.firstName },
                {"@lastname", newStaff.lastName },
                {"@fingerPrint", newStaff.fingerPrint },
                {"@haslunch", newStaff.hasLunch }
            };
            bool output = true;
            try
            {
                SqliteDataAccess.SaveData(sql, parameters);
            }
            catch
            {
                output = false;
            }
            return output;
            //bool output = true;
            //try
            //{
            //    using (var cnn = SqliteBaseRepository.SimpleDbConnection())
            //    {
            //        cnn.Open();
            //        var id = cnn.Query<long>(
            //                @"INSERT INTO staff(branch_id, firstName, lastName, hasLunch)
            //                VALUES(@branch_Id, @firstName, @lastName, @hasLunch)", newStaff).FirstOrDefault();
            //    }
            //}
            //catch
            //{
            //    output = false;
            //}
            //return output;
        }

        public bool DeleteStaff(int staffId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }
            string sql = "DELETE FROM staff WHERE id = @staffId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@staffId", staffId }
            };

            bool output = true;

            try
            {
                SqliteDataAccess.SaveData(sql, parameters);
            }
            catch
            {
                output = false;
            }
            return output;
            //bool output = true;
            //try
            //{
            //    using (var cnn = SqliteBaseRepository.SimpleDbConnection())
            //    {
            //        cnn.Open();
            //        var id = cnn.Query<long>(
            //                @"DELETE FROM staff WHERE id = @staffId", new { staffId }).FirstOrDefault();
            //    }
            //}
            //catch
            //{
            //    output = false;
            //}
            //return output;

        }
        public bool UpdateStaff(int id,StaffModel editedStaff)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }

            string sql = "UPDATE staff SET branch_id = @branch_id, firstName = @firstName, lastName = @lastName, hasLunch = @hasLunch, fingerPrint = @fingerPrint WHERE id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", editedStaff.branch_id},
                {"@firstName", editedStaff.firstName},
                {"@lastName", editedStaff.lastName},
                {"@hasLunch", editedStaff.hasLunch},
                {"@fingerPrint", editedStaff.fingerPrint },
                {"@id", id},
            };
            bool output = true;
            try
            {
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
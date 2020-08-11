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
        }
        public bool InsertStaff(StaffModel newStaff)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                SqliteBaseRepository.CreateDatabase();
            }
            string sql = "insert into staff(branch_id, firstname, lastname, fingerPrint) values(@branch_id, @firstname, @lastname, @fingerPrint)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", newStaff.branch_id },
                {"@firstname", newStaff.firstName },
                {"@lastname", newStaff.lastName },
                {"@fingerPrint", newStaff.fingerPrint }
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
        }
        public bool UpdateStaff(int id,StaffModel editedStaff)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }

            string sql;
            Dictionary<string, object> parameters;

            if (editedStaff.fingerPrint != null)
            {
                sql = "UPDATE staff SET branch_id = @branch_id, firstName = @firstName, lastName = @lastName, fingerPrint = @fingerPrint WHERE id = @id";

                parameters = new Dictionary<string, object>
                {
                    {"@branch_id", editedStaff.branch_id},
                    {"@firstName", editedStaff.firstName},
                    {"@lastName", editedStaff.lastName},
                    {"@fingerPrint", editedStaff.fingerPrint },
                    {"@id", id}
                };
            }
            else
            {
                sql = "UPDATE staff SET branch_id = @branch_id, firstName = @firstName, lastName = @lastName WHERE id = @id";

                parameters = new Dictionary<string, object>
                {
                    {"@branch_id", editedStaff.branch_id},
                    {"@firstName", editedStaff.firstName},
                    {"@lastName", editedStaff.lastName},
                    {"@id", id}
                };
            }
            
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
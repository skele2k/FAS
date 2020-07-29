using FASDataManager.Models;
using FASLib.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public class StaffRepository : IStaffRepository
    {
        // Todo: implement exceptions
        public List<StaffModel> GetStaffs(int amount, string sort)
        {
            string sql = "SELECT TOP @amount FROM staff ORDER BY id @sort";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@amount", amount},
                {"@sort", sort }
            };

            var output = SqliteDataAccess.LoadData<StaffModel>(sql, parameters);
            return output;
        }
        public StaffModel GetSingleStaff(int staffId)
        {
            string sql = "SELECT * FROM staff WHERE id = @staffId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@staffId", staffId }
            };

            var output = SqliteDataAccess.LoadData<StaffModel>(sql, parameters);
            return output[0];
        }
        public bool InsertStaff(StaffModel newStaff)
        {
            string sql = "INSERT INTO staff(branch_id, firstName, lastName, hasLunch) VALUES(@branch_Id, @firstName, @lastName, @hasLunch)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", newStaff.branch_id },
                {"@firstName", newStaff.firstName },
                {"@lastName", newStaff.lastName },
                {"@hasLunch", newStaff.hasLunch }
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
        public bool UpdateStaff(StaffModel editedStaff)
        {
            string sql = "UPDATE staff SET branch_id = @branch_id, firstName = @firstName, lastName = @lastName, fingerPrint = @fingerPrint, hasLunch = @hasLunch";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branch_id", editedStaff.branch_id},
                {"@firstName", editedStaff.firstName},
                {"@lastName", editedStaff.lastName},
                {"@fingerPrint", editedStaff.fingerPrint},
                {"@hasLunch", editedStaff.hasLunch}
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
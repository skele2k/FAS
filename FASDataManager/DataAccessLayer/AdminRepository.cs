using FASLib.DataAccess;
using FASLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public class AdminRepository : IAdminRepository
    {
        public List<AdminModel> GetAdmins()
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM admin";

            var output = SqliteDataAccess.LoadData<AdminModel>(sql, new Dictionary<string, object>());
            return output;
        }
        public AdminModel GetSingleAdmin(int adminId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM admin WHERE id = @adminId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@adminId", adminId }
            };

            var output = SqliteDataAccess.LoadData<AdminModel>(sql, parameters).FirstOrDefault();
            return output;
        }
        public bool InsertAdmin(AdminModel theAdmin)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                SqliteBaseRepository.CreateDatabase();
            }
            string sql = "INSERT INTO admin(username, password) VALUES(@username, @password)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@username", theAdmin.username},
                {"@password", theAdmin.password }
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
        public bool DeleteAdmin(int adminId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }
            string sql = "DELETE FROM admin WHERE id = @adminId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@adminId", adminId }
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
        public bool UpdateAdmin(int id, AdminModel theAdmin)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }

            string sql = "UPDATE admin SET username = @username, password = @password WHERE id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@username", theAdmin.username},
                {"@password", theAdmin.password},
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
using FASDataManager.Models;
using FASLib.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public class BranchRepository : IBranchRepository
    {
        public bool DeleteBranch(int branchId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }
            string sql = "DELETE FROM branch WHERE id = @branchId";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branchId", branchId }
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

        public BranchModel GetBranchByID(int branchId)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM branch WHERE id = @branchId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@branchId", branchId }
            };

            var output = SqliteDataAccess.LoadData<BranchModel>(sql, parameters).FirstOrDefault();
            return output;
        }

        public List<BranchModel> GetBranches()
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return null;
            }

            string sql = "SELECT * FROM branch";

            var output = SqliteDataAccess.LoadData<BranchModel>(sql, new Dictionary<string, object>());
            return output;
        }

        public bool InsertBranch(BranchModel theBranch)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                SqliteBaseRepository.CreateDatabase();
            }
            string sql = "INSERT INTO branch(name) VALUES(@name)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@name", theBranch.name }
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

        public bool UpdateBranch(int id,BranchModel theBranch)
        {
            if (!File.Exists(SqliteBaseRepository.DbLocation))
            {
                return false;
            }

            string sql = "UPDATE branch SET name = @name WHERE id = @id";

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@name", theBranch.name},
                {"@id", id}
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
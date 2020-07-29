using FASDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FASDataManager.Library.DataAccess
{
    public class AdminData
    {
        public List<AdminModel> GetAdminById(string Id)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new { Id = Id };
            var output = sql.LoadData<AdminModel, dynamic>("SELECT * FROM admin", p, "Default");

            return output;
        }
    }
}

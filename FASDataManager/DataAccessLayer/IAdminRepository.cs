using FASLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    internal interface IAdminRepository
    {
        List<AdminModel> GetAdmins();
        AdminModel GetSingleAdmin(int adminId);
        bool InsertAdmin(AdminModel theAdmin);
        bool DeleteAdmin(int adminId);
        bool UpdateAdmin(int id, AdminModel theAdmin);
    }
}
using FASDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASDataManager.DataAccessLayer
{
    internal interface IStaffRepository
    {
        List<StaffModel> GetStaffs();
        StaffModel GetSingleStaff(int staffId);
        bool InsertStaff(StaffModel ourCustomer);
        bool DeleteStaff(int staffId);
        bool UpdateStaff(int id, StaffModel ourCustomer);
    }
}

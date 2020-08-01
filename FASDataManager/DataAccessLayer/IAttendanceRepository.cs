using FASLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASDataManager.DataAccessLayer
{
    interface IAttendanceRepository
    {
        List<AttendanceModel> GetAttendance();
        List<AttendanceModel> GetAttendanceOfLastXDays(int X);
        bool AddStaffToAttendanceSheet(AttendanceModel theAttendance);
    }
}

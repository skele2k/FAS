using FASDataManager.DataAccessLayer;
using FASLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FASDataManager.Controllers
{
    public class AttendanceController : ApiController
    {
        AttendanceRepository _attendanceRepository = new AttendanceRepository();
        [Authorize]
        // GET: api/Attendance
        public List<AttendanceModel> Get()
        {
            return _attendanceRepository.GetAttendance();
        }

        [Authorize]
        // POST: api/Attendance
        public void Post([FromBody]AttendanceModel staffModel)
        {
            _attendanceRepository.AddStaffToAttendanceSheet(staffModel);
        }

        [Authorize]
        // DELETE: api/Attendance/5
        public void Delete(int id)
        {
        }
    }
}

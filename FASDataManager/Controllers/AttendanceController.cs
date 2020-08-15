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
    [Authorize]
    public class AttendanceController : ApiController
    {
        AttendanceRepository _attendanceRepository = new AttendanceRepository();
        // GET: api/Attendance
        public List<AttendanceModel> Get()
        {
            return _attendanceRepository.GetAttendance();
        }
        // POST: api/Attendance
        public bool Post([FromBody]AttendanceModel staffModel)
        {
            return _attendanceRepository.AddStaffToAttendanceSheet(staffModel);
        }
        // DELETE: api/Attendance/5
        public void Delete(int id)
        {
        }

        // PUT: api/Attendance
        public bool Put([FromBody]AttendanceModel attendanceModel)
        {
            return _attendanceRepository.EditAttendanceSheet(attendanceModel);
        }
    }
}

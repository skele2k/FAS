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
        /// <summary>
        /// Gets all attendances
        /// </summary>
        /// <returns>List of AttendanceModel</returns>
        public List<AttendanceModel> Get()
        {
            return _attendanceRepository.GetAttendance();
        }
        // POST: api/Attendance
        /// <summary>
        /// Add new AttendanceModel
        /// </summary>
        /// <param name="staffModel"></param>
        /// <returns>bool</returns>
        public bool Post([FromBody]AttendanceModel staffModel)
        {
            return _attendanceRepository.AddStaffToAttendanceSheet(staffModel);
        }

        // PUT: api/Attendance
        /// <summary>
        /// Replaces AttendanceModel according to the staff_id and branch_id from body
        /// </summary>
        /// <param name="attendanceModel"></param>
        /// <returns>bool</returns>
        public bool Put([FromBody]AttendanceModel attendanceModel)
        {
            return _attendanceRepository.EditAttendanceSheet(attendanceModel);
        }
    }
}

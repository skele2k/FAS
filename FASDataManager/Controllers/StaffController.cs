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
    public class StaffController : ApiController
    {
        private StaffRepository _staffRepository = new StaffRepository();
        // GET: api/Staff
        /// <summary>
        /// Get all the staffs
        /// </summary>
        /// <returns>List of StaffModel</returns>
        public List<StaffModel> Get()
        {
            return _staffRepository.GetStaffs();
        }

        // GET: api/Staff/5
        /// <summary>
        /// Get single staff by the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>StaffModel</returns>
        public StaffModel Get(int id)
        {
            return _staffRepository.GetSingleStaff(id);
        }

        // POST: api/Staff
        /// <summary>
        /// Add new staff
        /// </summary>
        /// <param name="theStaff"></param>
        /// <returns>bool</returns>
        public bool Post([FromBody]StaffModel theStaff)
        {
            return _staffRepository.InsertStaff(theStaff);
        }

        // PUT: api/Staff/5
        /// <summary>
        /// Replace existing staff with the staff from body by its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedStaff"></param>
        /// <returns>bool</returns>
        public bool Put(int id, [FromBody]StaffModel updatedStaff)
        {
            return _staffRepository.UpdateStaff(id,updatedStaff);
        }

        // DELETE: api/Staff/5
        /// <summary>
        /// Delete a staff by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public bool Delete(int id)
        {
            return _staffRepository.DeleteStaff(id);
        }
    }
}

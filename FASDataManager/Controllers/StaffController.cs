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
    public class StaffController : ApiController
    {
        private StaffRepository _staffRepository = new StaffRepository();
        // GET: api/Staff
        public List<StaffModel> Get()
        {
            return _staffRepository.GetStaffs();
        }

        // GET: api/Staff/5
        public StaffModel Get(int id)
        {
            return _staffRepository.GetSingleStaff(id);
        }
        // POST: api/Staff
        public bool Post([FromBody]StaffModel theStaff)
        {
            return _staffRepository.InsertStaff(theStaff);
        }

        // PUT: api/Staff/5
        public bool Put(int id, [FromBody]StaffModel updatedStaff)
        {
            return _staffRepository.UpdateStaff(id,updatedStaff);
        }

        // DELETE: api/Staff/5
        public bool Delete(int id)
        {
            return _staffRepository.DeleteStaff(id);
        }
    }
}

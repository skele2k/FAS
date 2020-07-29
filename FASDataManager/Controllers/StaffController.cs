using FASDataManager.DataAccessLayer;
using FASDataManager.Models;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Staff/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        // POST: api/Staff
        public bool Post([FromBody]StaffModel theStaff)
        {
            return _staffRepository.InsertStaff(theStaff);
        }

        // PUT: api/Staff/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Staff/5
        public void Delete(int id)
        {
        }
    }
}

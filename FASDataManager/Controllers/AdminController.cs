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
    public class AdminController : ApiController
    {
        AdminRepository _adminRepository = new AdminRepository();
        [Authorize]
        // GET: api/Admin
        public List<AdminModel> Get()
        {
            return _adminRepository.GetAdmins();
        }

        [Authorize]
        // GET: api/Admin/5
        public AdminModel Get(int id)
        {
            return _adminRepository.GetSingleAdmin(id);
        }

        [Authorize]
        // POST: api/Admin
        public void Post([FromBody]AdminModel theAdmin)
        {
            _adminRepository.InsertAdmin(theAdmin);
        }

        [Authorize]
        // PUT: api/Admin/5
        public void Put(int id, [FromBody]AdminModel newAdmin)
        {
            _adminRepository.UpdateAdmin(id, newAdmin);
        }

        [Authorize]
        // DELETE: api/Admin/5
        public void Delete(int id)
        {
            _adminRepository.DeleteAdmin(id);
        }
    }
}

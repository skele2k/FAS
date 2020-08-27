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
    public class AdminController : ApiController
    {
        AdminRepository _adminRepository = new AdminRepository();
        // GET: api/Admin
        /// <summary>
        /// Gets all Admins
        /// </summary>
        /// <returns>List of AdminModel</returns>
        public List<AdminModel> Get()
        {
            return _adminRepository.GetAdmins();
        }

        // GET: api/Admin/5
        /// <summary>
        /// Gets single admin corresponding to the id number.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AdminModel</returns>
        public AdminModel Get(int id)
        {
            return _adminRepository.GetSingleAdmin(id);
        }

        // POST: api/Admin
        /// <summary>
        /// Insert new admin
        /// </summary>
        /// <param name="theAdmin"></param>
        public void Post([FromBody]AdminModel theAdmin)
        {
            _adminRepository.InsertAdmin(theAdmin);
        }

        // PUT: api/Admin/id
        /// <summary>
        /// Edits current admin's username and password by the id number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newAdmin"></param>
        public void Put(int id, [FromBody]AdminModel newAdmin)
        {
            _adminRepository.UpdateAdmin(id, newAdmin);
        }

        // DELETE: api/Admin/5
        /// <summary>
        /// Delete the admin by id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _adminRepository.DeleteAdmin(id);
        }
    }
}

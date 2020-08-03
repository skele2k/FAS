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
    public class BranchController : ApiController
    {
        private BranchRepository _branchRepository = new BranchRepository();

        [Authorize]
        // GET: api/Branch
        public List<BranchModel> Get()
        {
            return _branchRepository.GetBranches();
        }

        [Authorize]
        // GET: api/Branch/5
        public BranchModel Get(int id)
        {
            return _branchRepository.GetBranchByID(id);
        }

        [Authorize]
        // POST: api/Branch
        public bool Post([FromBody]BranchModel theBranch)
        {
            return _branchRepository.InsertBranch(theBranch);
        }

        [Authorize]
        // PUT: api/Branch/5
        public bool Put(int id, [FromBody]BranchModel theBranch)
        {
            return _branchRepository.UpdateBranch(id, theBranch);
        }

        [Authorize]
        // DELETE: api/Branch/5
        public bool Delete(int id)
        {
            return _branchRepository.DeleteBranch(id);
        }
    }
}

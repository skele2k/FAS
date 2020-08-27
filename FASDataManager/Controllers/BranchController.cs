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
    public class BranchController : ApiController
    {
        private BranchRepository _branchRepository = new BranchRepository();

        // GET: api/Branch
        /// <summary>
        /// Get all the branches
        /// </summary>
        /// <returns>List of BranchModel</returns>
        public List<BranchModel> Get()
        {
            return _branchRepository.GetBranches();
        }

        // GET: api/Branch/5
        /// <summary>
        /// Get a branch by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>BranchModel</returns>
        public BranchModel Get(int id)
        {
            return _branchRepository.GetBranchByID(id);
        }

        // POST: api/Branch
        /// <summary>
        /// Add new branch
        /// </summary>
        /// <param name="theBranch"></param>
        /// <returns>bool</returns>
        public bool Post([FromBody]BranchModel theBranch)
        {
            return _branchRepository.InsertBranch(theBranch);
        }

        // PUT: api/Branch/5
        /// <summary>
        /// Replace existing branch with the branch from body by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="theBranch"></param>
        /// <returns>bool</returns>
        public bool Put(int id, [FromBody]BranchModel theBranch)
        {
            return _branchRepository.UpdateBranch(id, theBranch);
        }

        // DELETE: api/Branch/5
        /// <summary>
        /// Delete a branch by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        public bool Delete(int id)
        {
            return _branchRepository.DeleteBranch(id);
        }
    }
}

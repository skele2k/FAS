using FASDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASDataManager.DataAccessLayer
{
    internal interface IBranchRepository
    {
         List<BranchModel> GetBranches();
         BranchModel GetBranchByID(int branchId);
         bool InsertBranch(BranchModel theBranch);
         bool DeleteBranch(int branchId);
         bool UpdateBranch(int id,BranchModel theBranch);
    }
}

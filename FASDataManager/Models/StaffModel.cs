using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASDataManager.Models
{
    public class StaffModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fingerPrint { get; set; }
        public int hasLunch { get; set; }
    }
}
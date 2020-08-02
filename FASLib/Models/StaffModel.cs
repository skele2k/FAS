using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASLib.Models
{
    public class StaffModel
    {
        public int id { get; set; }
        public int branch_id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public byte[] fingerPrint { get; set; }
        public int hasLunch { get; set; }
        public string fullName
        {
            get
            {
                return lastName + " " + firstName;
            }
        }
    }
}

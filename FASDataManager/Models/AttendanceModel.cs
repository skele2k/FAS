using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASDataManager.Models
{
    public class AttendanceModel
    {
        public int staff_id { get; set; }
        public int branch_id { get; set; }
        public string date { get; set; }
        public bool hasLunch { get; set; }
        public string arriveTime { get; set; }
        public string leaveTime { get; set; }
        public double officeHours { get; set; }
        public int atOffice { get; set; }
    }
}
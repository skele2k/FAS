using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public static class DBPathFinder
    {
        public static string findDB()
        {
            var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/AssemFiles/AttendDB.sqlite");
            return fullPath;
        }
    }
}
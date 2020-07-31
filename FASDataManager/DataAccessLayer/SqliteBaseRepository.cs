using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace FASDataManager.DataAccessLayer
{
    public class SqliteBaseRepository
    {
        public static string DbFile
        {
            //get { return Environment.CurrentDirectory + "\\AttendDB.sqlite";  }
            get 
            {
                string output = "|DataDirectory|\\AttendDB.sqlite";
                return output;
            }
        }
        public static string DbLocation
        {
            get
            {
                var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/AttendDB.sqlite");
                return fullPath;
            }
        }
        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile + ";Version=3;");
        }

        public static void CreateDatabase()
        {
            using (var cnn = SqliteBaseRepository.SimpleDbConnection())
            {
                cnn.Open();
                cnn.Execute(
                    @"CREATE TABLE staff
                    (
                        id              INTEGER NOT NULL UNIQUE,
                        branch_id       INTEGER,
                        firstName       TEXT NOT NULL,
                        lastName        TEXT NOT NULL,
                        fingerPrint     TEXT,
                        hasLunch        INTEGER NOT NULL,
                        FOREIGN KEY(branch_id) REFERENCES branch(id) ON DELETE SET NULL,
                        PRIMARY KEY(id AUTOINCREMENT)
                    );

                    CREATE TABLE branch
                    (
                        id              INTEGER NOT NULL UNIQUE,
                        name            TEXT NOT NULL,
                        PRIMARY KEY(id AUTOINCREMENT)
                    );
                    
                    CREATE TABLE attendance
                    (
                        staff_id        INTEGER NOT NULL,
                        branch_id       INTEGER NOT NULL,
                        date            TEXT NOT NULL,
                        hasLunch        INTEGER,
                        arriveTime      TEXT,
                        leaveTime       TEXT,
                        officeHours     REAL DEFAULT 0,
                        atOffice        INTEGER NOT NULL DEFAULT 0,
                        FOREIGN KEY(staff_id) REFERENCES staff(id) ON DELETE CASCADE,
                        FOREIGN KEY(branch_id) REFERENCES branch(id) ON DELETE SET NULL
                    );
                    
                    CREATE TABLE admin
                    (
                        id              INTEGER NOT NULL UNIQUE,
                        username        TEXT NOT NULL,
                        password        TEXT NOT NULL,
                        PRIMARY KEY(id AUTOINCREMENT)
                    );
                ");

            }
        }
    }
}
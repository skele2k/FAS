using Dapper;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASLib.DataAccess
{
    public static class SqliteBaseRepository
    {
        public static string FullPath { get; set; }
        public static string DbLocation
        {
            get
            {
                string untilUser = System.Environment.GetEnvironmentVariable("USERPROFILE");
                string fullPath = untilUser + @"\AppData\Local\FAS\db";
                System.IO.Directory.CreateDirectory(fullPath);
                return fullPath + @"\AttendDB.sqlite";
            }
        }
        public static SQLiteConnection SimpleDbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbLocation + "; Version = 3;");
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
                        fingerPrint     BLOB,
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
                    
                    INSERT INTO admin(username, password) VALUES('admin', 'admin');
                ");

            }
        }
    }
}

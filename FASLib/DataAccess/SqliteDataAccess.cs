﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASLib.DataAccess
{
    public static class SqliteDataAccess
    {
        public static List<T> LoadData<T>(string sqlStatement, Dictionary<string, object> parameters, string connectionName = "Default")
        {
            DynamicParameters p = parameters.ToDynamicParameters();

            using (IDbConnection cnn = SqliteBaseRepository.SimpleDbConnection())
            {
                var rows = cnn.Query<T>(sqlStatement, p);

                return rows.ToList();
            }
        }

        public static void SaveData(string sqlStatement, Dictionary<string, object> parameters, string connectionName = "Default")
        {
            DynamicParameters p = parameters.ToDynamicParameters();

            using (IDbConnection cnn = SqliteBaseRepository.SimpleDbConnection())
            {
                cnn.Execute(sqlStatement, p);
            }
        }

        // Turns Dictionary to Dynamic Parameters.
        private static DynamicParameters ToDynamicParameters(this Dictionary<string, object> p)
        {
            DynamicParameters output = new DynamicParameters();
            foreach (var param in p)
            {
                //         @FirstName, "Tim"
                output.Add(param.Key, param.Value);
            }

            return output;
        }
    }
}

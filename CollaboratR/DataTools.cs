using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CollaboratR
{
    public static class DataTools
    {
        /// <summary>
        /// Run a stored procedure defined by a SqlCommand object
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>A Dataset containing any information retrieved from the database</returns>
        public static DataSet RunStoredProcedure(SqlCommand cmd)
        {
            DataSet dsResults = new DataSet();
            cmd.CommandType = CommandType.StoredProcedure;
            using(cmd) //using-statement invokes the dispose method after leaving scope
            {
                using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dsResults);
                }
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return dsResults;
        }
        public static DataSet RunStoredProcedure(SqlCommand cmd, String connectionString)
        {
            cmd.Connection = new SqlConnection(connectionString);
            return RunStoredProcedure(cmd);
        }

        public static DataSet RunQuery(SqlCommand cmd)
        {
            DataSet dsResults = new DataSet();
            using(cmd)
            {
                using(SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dsResults);
                }
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
            return dsResults;
        }

        public static DataSet RunQuery(String command, String connectionString)
        {
            SqlCommand cmd = new SqlCommand(command, new SqlConnection(connectionString));
            return RunQuery(cmd);
        }
    }
}
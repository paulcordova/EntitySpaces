using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleDatabase : Database
	{
		public OracleDatabase()
		{

		}

        public DataTable ExecuteSql(string sql)
        {
            // Create the Oracle connection using ODP.NET
            OracleConnection cn = new OracleConnection(dbRoot.ConnectionString);
            cn.Open();

            // Execute the SQL command and return a DataTable
            DataTable result = this.ExecuteIntoDataTable(sql, cn);

            // Close the connection
            cn.Close();

            return result;
        }

        private DataTable ExecuteIntoDataTable(string sql, OracleConnection connection)
        {
            // Create an OracleCommand to execute the SQL query
            using (OracleCommand cmd = new OracleCommand(sql, connection))
            {
                // Use OracleDataAdapter to fill a DataTable with the query results
                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);  // Populate the DataTable with data
                    return dt;  // Return the populated DataTable
                }
            }
        }

        public override string Name
		{
			get
			{
				object o = _row["SCHEMA_NAME"];

				if(DBNull.Value == o)
				{
					return string.Empty;
				}
				else
				{
					return (string)o;
				}
			}
		}

	}
}

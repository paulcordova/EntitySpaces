using System;
using System.Data;
using System.Data.Common;
using OracleClient = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleTables : Tables
	{
		public OracleTables()
		{

		}

      
        override internal void LoadAll()
        {
            try
            {
                string query = @"SELECT OWNER, table_name" +
                                " FROM all_tables" +
                                " WHERE owner = :schema" +
                                " ORDER BY table_name";
                                                

                // Create a DataTable to hold the metadata
                DataTable metaData = new DataTable();

                // Execute the query using ODP.NET
                using (OracleClient.OracleConnection cn = new OracleClient.OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleClient.OracleCommand cmd = new OracleClient.OracleCommand(query, cn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.Add(new OracleClient.OracleParameter(this.Database.SchemaOwner, this.Database.SchemaOwner));

                        // Use OracleDataAdapter to fill the DataTable with the query results
                        using (OracleClient.OracleDataAdapter adapter = new OracleClient.OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Populate the array or internal structure with the retrieved metadata
                PopulateArray(metaData);

                //LoadExtraData(this.Database.SchemaName);
                LoadExtraData(this.Database.SchemaOwner);

            }
            catch
            {
                // Handle exceptions as needed, potentially logging them or alerting the user
            }
        }


        private void LoadExtraData(string schema)
        {
            try
            {
                string select = "SELECT DISTINCT C.TABLE_NAME, C.COMMENTS AS DESCRIPTION" + 
                                " FROM SYS.ALL_TAB_COMMENTS C, SYS.ALL_TABLES T" +
                                " WHERE T.OWNER = :schema  AND" +
                                " T.OWNER = C.OWNER	AND" + 
                                " T.TABLE_NAME = C.TABLE_NAME AND" +
                                " C.COMMENTS IS NOT NULL";

                // Create a DataTable to hold the metadata
                DataTable metaData = new DataTable();

                // Execute the query using ODP.NET
                using (OracleClient.OracleConnection cn = new OracleClient.OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleClient.OracleCommand cmd = new OracleClient.OracleCommand(select, cn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.Add(new OracleClient.OracleParameter(schema, schema));

                        // Use OracleDataAdapter to fill the DataTable with the query results
                        using (OracleClient.OracleDataAdapter adapter = new OracleClient.OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                DataRowCollection rows = metaData.Rows;

                if (rows.Count > 0)
                {
                    Table t;
                    foreach (DataRow row in rows)
                    {
                        t = this[row["TABLE_NAME"]] as Table;
                        t._row["DESCRIPTION"] = row["DESCRIPTION"];
                    }
                }
            }
            catch { }
        }

       

    }
}

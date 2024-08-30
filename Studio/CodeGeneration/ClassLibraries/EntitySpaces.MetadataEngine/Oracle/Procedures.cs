using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleProcedures : Procedures
	{
		public OracleProcedures()
		{

		}

     
        override internal void LoadAll()
        {
            try
            {
                // Create a DataTable to hold procedure metadata
                DataTable metaData = new DataTable();

                // SQL query to retrieve procedure metadata
                string query = "SELECT * FROM ALL_PROCEDURES WHERE " + "" +
                                " OBJECT_OWNER = '" + Database.SchemaOwner + "'";
                

                // Execute the query and fill the DataTable
                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        // Add parameter to the command
                        cmd.Parameters.Add(new OracleParameter(this.Database.Name));

                        // Fill the DataTable with the query result
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Populate the array with the procedure metadata
                PopulateArray(metaData);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                Console.WriteLine(ex.Message);
            }
        }
    }
}

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleParameters : Parameters
	{
		public OracleParameters()
		{

		}
       
        override internal void LoadAll()
        {
            try
            {
                // Create a DataTable to hold procedure parameters metadata
                DataTable metaData = new DataTable();

                // SQL query to retrieve procedure parameters metadata
                string query = @"SELECT * 
                                FROM ALL_ARGUMENTS 
                                WHERE OBJECT_OWNER = :databaseOwner 
                                  AND OBJECT_NAME = :procedureName 
                                  AND OBJECT_TYPE = 'PROCEDURE'
                                ORDER BY SEQUENCE";

                // Execute the query and fill the DataTable
                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.Add(new OracleParameter(this.Procedure.Database.Name));
                        cmd.Parameters.Add(new OracleParameter(this.Procedure.Name));

                        // Fill the DataTable with the query result
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Populate the array with the procedure parameters metadata
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

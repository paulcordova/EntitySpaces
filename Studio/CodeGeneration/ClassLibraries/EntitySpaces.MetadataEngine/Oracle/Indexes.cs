using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleIndexes : Indexes
	{
		public OracleIndexes()
		{

		}


        override internal void LoadAll()
        {
            try
            {
                // Create a DataTable to hold index metadata
                DataTable metaData = new DataTable();

                // SQL query to retrieve index metadata
                string query = "SELECT * FROM ALL_INDEXES WHERE " +
                                " TABLE_OWNER = '" + this.Table.Database.SchemaOwner + "' AND " +
                                " TABLE_NAME = '" + this.Table.Name + "' ";

                // Execute the query and fill the DataTable
                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                       
                        // Fill the DataTable with the query result
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Populate the array with the index metadata
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

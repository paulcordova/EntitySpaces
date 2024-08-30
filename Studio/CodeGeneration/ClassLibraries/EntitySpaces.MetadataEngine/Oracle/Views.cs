using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleViews : Views
	{
		public OracleViews()
		{

		}

       
        internal override void LoadAll()
        {
            try
            {
                // Determine the type of views to retrieve
                string type = this.dbRoot.ShowSystemData ? "SYSTEM VIEW" : "VIEW";

                // SQL query to retrieve views metadata
                string query = "SELECT * FROM ALL_VIEWS WHERE OWNER = :owner";

                // Create a DataTable to hold the results
                DataTable metaData = new DataTable();

                // Execute the query using ODP.NET
                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.Add(new OracleParameter(this.Database.Name));

                        // Use OracleDataAdapter to fill the DataTable with the query results
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Populate the base array with the metadata
                base.PopulateArray(metaData);

                // Load extra data about views
                LoadExtraData(this.Database.SchemaName);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                Console.WriteLine(ex.Message);
            }
        }

        private void LoadExtraData(string schema)
        {
            try
            {
                // SQL query to retrieve comments for views
                string select = "SELECT DISTINCT C.TABLE_NAME, C.COMMENTS AS DESCRIPTION FROM SYS.ALL_TAB_COMMENTS C " +
                    "JOIN SYS.ALL_VIEWS V ON V.OWNER = C.OWNER AND V.VIEW_NAME = C.TABLE_NAME " +
                    " WHERE V.OWNER = '" +  Database.SchemaOwner + "'" +
                    " AND C.COMMENTS IS NOT NULL";

                // Create a DataTable to hold the results
                DataTable dataTable = new DataTable();

                // Execute the query using ODP.NET
                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(select, cn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.Add(new OracleParameter(schema));

                        // Use OracleDataAdapter to fill the DataTable with the query results
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }

                // Process the results
                if (dataTable.Rows.Count > 0)
                {
                    View v;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Find the View object by its name
                        v = this[row["TABLE_NAME"]] as View;
                        if (v != null)
                        {
                            // Update the description
                            v._row["DESCRIPTION"] = row["DESCRIPTION"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                Console.WriteLine(ex.Message);
            }
        }

    }
}

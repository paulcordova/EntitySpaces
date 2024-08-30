using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleForeignKeys : ForeignKeys
	{
		public OracleForeignKeys()
		{

		}
        		

override internal void LoadAll()
    {
        try
        {
            // Create a DataTable to hold foreign key metadata
            DataTable metaData1 = new DataTable();
            DataTable metaData2 = new DataTable();

            // SQL queries to retrieve foreign key metadata
            string query1 = "SELECT * FROM ALL_CONS_COLUMNS WHERE TABLE_NAME = '" + this.Table.Name + "' AND CONSTRAINT_NAME IN " +
                            "(SELECT CONSTRAINT_NAME FROM ALL_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME = '" +  this.Table.Name + "')";

            string query2 = "SELECT * FROM ALL_CONS_COLUMNS WHERE CONSTRAINT_NAME IN " +
                            "(SELECT CONSTRAINT_NAME FROM ALL_CONSTRAINTS WHERE CONSTRAINT_TYPE = 'R' AND TABLE_NAME = '" + this.Table.Name + "')";

            // Execute queries and fill DataTables
            using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
            {
                cn.Open();

                // Execute the first query
                using (OracleCommand cmd = new OracleCommand(query1, cn))
                {
                    //cmd.Parameters.Add(new OracleParameter(this.Table.Name));
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(metaData1);
                    }
                }

                // Execute the second query
                using (OracleCommand cmd = new OracleCommand(query2, cn))
                {
                    //cmd.Parameters.Add(new OracleParameter(this.Table.Name));
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(metaData2);
                    }
                }
            }

            // Import rows from the second DataTable into the first
            foreach (DataRow row in metaData2.Rows)
            {
                metaData1.ImportRow(row);
            }

            // Populate the array with the combined metadata
            PopulateArray(metaData1);
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., log the error
            Console.WriteLine(ex.Message);
        }
    }


}
}

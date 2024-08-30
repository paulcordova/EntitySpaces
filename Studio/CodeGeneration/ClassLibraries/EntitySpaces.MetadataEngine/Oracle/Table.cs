using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleTable : Table
	{
		public OracleTable()
		{

		}

        public override IColumns PrimaryKeys
        {
            get
            {
                if (null == _primaryKeys)
                {
                    // Initialize the _primaryKeys collection
                    _primaryKeys = (Columns)this.dbRoot.ClassFactory.CreateColumns();
                    _primaryKeys.Table = this;
                    _primaryKeys.dbRoot = this.dbRoot;

                    // Create the Oracle connection using ODP.NET
                    using (OracleConnection cn = new OracleConnection(dbRoot.ConnectionString))
                    {
                        cn.Open();

                        string dbNAme = Database.SchemaOwner;

                        // Query to get primary keys metadata using ODP.NET
                        string query =
                            " SELECT cols.COLUMN_NAME" +
                            " FROM ALL_CONS_COLUMNS cols" +
                            " JOIN ALL_CONSTRAINTS cons ON cols.CONSTRAINT_NAME = cons.CONSTRAINT_NAME" +
                            " WHERE cons.CONSTRAINT_TYPE = 'P'" +
                                " AND cols.OWNER = '" + Database.SchemaOwner + "'" +
                                " AND cols.TABLE_NAME = '" + this.Name + "'";
                        

                        // Create an OracleCommand to execute the query
                        using (OracleCommand cmd = new OracleCommand(query, cn))
                        {
                            // Use OracleDataAdapter to fill a DataTable with the results
                            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                            {
                                DataTable metaData = new DataTable();
                                adapter.Fill(metaData);  // Populate the DataTable with data

                                Columns cols = (Columns)this.Columns;
                                string colName = "";

                                int count = metaData.Rows.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    colName = metaData.Rows[i]["COLUMN_NAME"] as string;
                                    _primaryKeys.AddColumn((Column)cols.GetByPhysicalName(colName));
                                }
                            }
                        }
                    }
                }

                return _primaryKeys;
            }
        }


        
        private DataTable LoadData(string query, params global::Oracle.ManagedDataAccess.Client.OracleParameter[] parameters)
    {
        // Create the Oracle connection using ODP.NET
        using (OracleConnection cn = new OracleConnection(dbRoot.ConnectionString))
        {
            cn.Open();

            // Create an OracleCommand to execute the query
            using (OracleCommand cmd = new OracleCommand(query, cn))
            {
                // Add parameters to the command
                cmd.Parameters.AddRange(parameters);

                // Use OracleDataAdapter to fill a DataTable with the results
                using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);  // Populate the DataTable with data
                    return dt;  // Return the populated DataTable
                }
            }
        }
    }






}
}

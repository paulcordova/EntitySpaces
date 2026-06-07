using System;
using System.Data;
using System.Data.Common;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleColumns : Columns
	{
		public OracleColumns()
		{

		}

        internal  DataColumn f_TypeName;
        internal DataColumn f_TypeNameComplete;

        override internal void LoadForTable()
        {
            IDbConnection cn = null;

            try
            {
                // Query to retrieve column information for the specified table in Oracle
                string query = @"SELECT " +
                " COLUMN_NAME, " +
                " DATA_TYPE AS TYPE_NAME, " +
                " DATA_TYPE AS TYPE_NAMECOMPLETE, " +
                " DATA_LENGTH AS CHARACTER_MAXIMUM_LENGTH, " +
                " DATA_PRECISION AS NUMERIC_PRECISION, " +    // Added
                " DATA_SCALE AS NUMERIC_SCALE, " +            // Added
                " CASE " +
                "    WHEN NULLABLE = 'Y' THEN '1' " +
                "    ELSE '0' " +
                " END AS IS_NULLABLE, " +
                " DATA_DEFAULT AS COLUMN_DEFAULT, " +
                " COLUMN_ID AS ORDINAL_POSITION " +
                " FROM ALL_TAB_COLUMNS " +
                " WHERE owner = '" + this.Table.Database.SchemaOwner + "'" +
                " AND TABLE_NAME = '" + this.Table.FullName + "'" +
                " ORDER BY COLUMN_ID";


                // Fill DataTable with metadata
                DataTable metaData = new DataTable();

                // Create the connection using Oracle.ManagedDataAccess
                //using (OracleClient.OracleConnection cn = new OracleClient.OracleConnection(this.dbRoot.ConnectionString))
                using (cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name))
                {

                    DbDataAdapter adapter = new OracleDataAdapter(query, (OracleConnection)cn);
                    adapter.Fill(metaData);

                    // Rename columns to match the existing code expectations
                    if (metaData.Columns.Contains("TYPE_NAME"))
                        f_TypeName = metaData.Columns["TYPE_NAME"];

                    if (metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
                        f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];


                    // Populate the array with the metadata
                    PopulateArray(metaData);
                }
               

                // Logic to identify and process auto-increment columns (AutoKey)
                query =
                    "SELECT " +
                    "    cols.COLUMN_NAME, " +
                    "    seq.SEQUENCE_NAME " +
                    "FROM " +
                    "    ALL_TAB_COLUMNS cols " +
                    "    JOIN ALL_SEQUENCES seq ON cols.OWNER = seq.SEQUENCE_OWNER " +
                    "    AND cols.TABLE_NAME = seq.SEQUENCE_NAME " +
                    "WHERE " +
                    "    cols.owner = '" + this.Table.Database.SchemaOwner + "'" +
                    "    AND cols.TABLE_NAME = '" + this.Table.Name + "' " +
                    "    AND cols.IDENTITY_COLUMN = 'YES'";

                using (cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name))
                {
                    DataTable seqData = new DataTable();
                    DbDataAdapter adapter = new OracleDataAdapter(query, (OracleConnection)cn);

                    adapter = new OracleDataAdapter(query, (OracleConnection)cn);
                    adapter.Fill(seqData);

                    DataRowCollection rows = seqData.Rows;

                    if (rows.Count > 0)
                    {
                        for (int i = 0; i < rows.Count; i++)
                        {
                            string colName = rows[i]["COLUMN_NAME"] as string;

                            // Assuming you have an Oracle-specific column class like PostgreSQLColumn
                            OracleColumn col = this[colName] as OracleColumn;
                            col._isAutoKey = true;

                            // Query to get the sequence details
                            query =
                                "SELECT MIN_VALUE, INCREMENT_BY" +
                                " FROM ALL_SEQUENCES " +
                                " WHERE SEQUENCE_NAME = '" + rows[i]["SEQUENCE_NAME"] + "'";

                            DataTable autokeyData = new DataTable();
                            adapter = new OracleDataAdapter(query, (OracleConnection)cn);
                            adapter.Fill(autokeyData);

                            Int64 a;

                            a = Convert.ToInt64(autokeyData.Rows[0]["MIN_VALUE"]);
                            col._autoInc = Convert.ToInt32(a);

                            a = Convert.ToInt64(autokeyData.Rows[0]["INCREMENT_BY"]);
                            col._autoSeed = Convert.ToInt32(a);
                        }
                    }
                }
            }
            catch
            {
                if (cn != null)
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }


        override internal void LoadForView()
        {
            try
            {
                // Determine the schema owner and table name dynamically
                // If this.Table is null, we are dealing with a View
                string schemaOwner = (this.Table != null) ? this.Table.Database.SchemaOwner : this.View.Database.SchemaOwner;
                string objectName = (this.Table != null) ? this.Table.Name : this.View.Name;

                // Query to retrieve column information
                // We use the variables defined above to avoid NullReferenceException
                string query = @"SELECT " +
               "    column_name AS COLUMN_NAME, " +
               "    data_type AS TYPE_NAME, " +
               "    data_type AS TYPE_NAMECOMPLETE, " +
               "    data_length AS CHARACTER_MAXIMUM_LENGTH, " +
               "    data_precision AS NUMERIC_PRECISION, " +  // Added
               "    data_scale AS NUMERIC_SCALE, " +          // Added
               "    nullable AS IS_NULLABLE, " +
               "    data_default AS COLUMN_DEFAULT, " +
               "    column_id AS ORDINAL_POSITION " +
               "FROM all_tab_columns " +
               "WHERE owner = '" + schemaOwner + "' " +
               "AND table_name = '" + objectName + "' " +
               "ORDER BY column_id";

                // ConnectionHelper needs to know which database name to use
                string dbName = (this.Table != null) ? this.Table.Database.Name : this.View.Database.Name;

                using (IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, dbName))
                {
                    DataTable metaData = new DataTable();

                    // Using explicit cast to OracleConnection
                    using (OracleDataAdapter adapter = new OracleDataAdapter(query, (OracleConnection)cn))
                    {
                        adapter.Fill(metaData);
                    }

                    if (metaData.Columns.Contains("TYPE_NAME"))
                        f_TypeName = metaData.Columns["TYPE_NAME"];

                    if (metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
                        f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];

                    PopulateArray(metaData);
                }
            }
            catch (Exception ex)
            {
                // Log the exception to understand what's failing if it still does
                Console.WriteLine("Error in LoadForView: " + ex.Message);
            }
        }

    } // end class
} // end namespace

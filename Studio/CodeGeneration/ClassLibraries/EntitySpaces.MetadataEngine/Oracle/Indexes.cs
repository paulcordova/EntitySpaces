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
                // Step 1 — load index headers
                string queryIndexes =
                    "SELECT i.INDEX_NAME, i.INDEX_TYPE, i.UNIQUENESS, i.TABLE_NAME " +
                    "FROM ALL_INDEXES i " +
                    "WHERE i.TABLE_OWNER = '" + this.Table.Database.SchemaOwner + "' " +
                    "  AND i.TABLE_NAME  = '" + this.Table.Name + "' " +
                    "ORDER BY i.INDEX_NAME";

                DataTable metaData = new DataTable();

                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(queryIndexes, cn))
                    using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                    {
                        adapter.Fill(metaData);
                    }
                }

                // Use NoHookup so we can wire columns manually below
                PopulateArrayNoHookup(metaData);

                // Step 2 — for each index, load its columns and call AddColumn
                // (same pattern as PostgreSQL provider)
                for (int i = 0; i < this.Count; i++)
                {
                    Index index = this[i] as Index;
                    if (index == null) continue;

                    string queryColumns =
                        "SELECT COLUMN_NAME " +
                        "FROM ALL_IND_COLUMNS " +
                        "WHERE INDEX_OWNER = '" + this.Table.Database.SchemaOwner + "' " +
                        "  AND INDEX_NAME  = '" + index.Name + "' " +
                        "ORDER BY COLUMN_POSITION";

                    using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                    {
                        cn.Open();

                        using (OracleCommand cmd = new OracleCommand(queryColumns, cn))
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            DataTable colData = new DataTable();
                            adapter.Fill(colData);

                            foreach (DataRow row in colData.Rows)
                            {
                                string colName = row["COLUMN_NAME"] as string;
                                if (!string.IsNullOrEmpty(colName))
                                {
                                    index.AddColumn(colName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteColumns : Columns
	{
		public SQLiteColumns()
		{

		}

		internal DataColumn f_TypeName = null;
		internal DataColumn f_TypeNameComplete	= null;

        override internal void LoadForTable()
        {
            IDbConnection cn = null;

            try
            {
                cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name);

                // SQLite does not have information_schema — use pragma_table_info instead
                // Returns: cid | name | type | notnull | dflt_value | pk
                string query = $"PRAGMA table_info(\"{this.Table.Name}\")";

                DataTable metaData = new DataTable();
                DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, cn);
                adapter.Fill(metaData);

                // Clone 'type' into TYPE_NAMECOMPLETE before renaming,
                // since SQLite has no separate udt_name column
                DataColumn typeClone = new DataColumn("TYPE_NAMECOMPLETE");
                metaData.Columns.Add(typeClone);
                foreach (DataRow row in metaData.Rows)
                    row["TYPE_NAMECOMPLETE"] = row["type"];

                metaData.Columns["type"].ColumnName = "TYPE_NAME";

                f_TypeName = metaData.Columns["TYPE_NAME"];
                f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];

                PopulateArray(metaData);

                // Detect auto-increment columns via sqlite_master DDL inspection.
                // SQLite has two equivalent forms of auto-increment:
                //   Form 1: INTEGER PRIMARY KEY            (rowid alias, no keyword needed)
                //   Form 2: INTEGER PRIMARY KEY AUTOINCREMENT (explicit keyword, stricter)
                // Both behave as auto-increment for EntitySpaces purposes.
                // The original code only detected Form 2, missing most real-world tables
                // (Northwind included) which use Form 1 without the explicit keyword.
                DataTable masterData = new DataTable();
                adapter = SQLiteDatabases.CreateAdapter(
                    $"SELECT sql FROM sqlite_master WHERE type='table' AND name='{this.Table.Name}'", cn);
                adapter.Fill(masterData);

                string tableDdl = masterData.Rows.Count > 0
                    ? masterData.Rows[0]["sql"]?.ToString() ?? string.Empty
                    : string.Empty;

                // Count PK columns from pragma result (pk > 0 means the column is part of the PK)
                int pkColCount = 0;
                foreach (DataRow r in metaData.Rows)
                    if (Convert.ToInt32(r["pk"]) > 0) pkColCount++;

                // Re-use already loaded metaData (pragma_table_info) to find PK columns.
                // Mark a column as auto-increment when ANY of the following is true:
                //   a) DDL contains the AUTOINCREMENT keyword (Form 2)
                //   b) Single-column PK whose type is exactly INTEGER â€” SQLite rowid alias (Form 1)
                //      Note: ROWID alias requires the type to be literally "INTEGER" (not "INT",
                //      not "INT4", etc.). We match case-insensitively to be safe.
                foreach (DataRow pkRow in metaData.Rows)
                {
                    int pkIndex = Convert.ToInt32(pkRow["pk"]);
                    if (pkIndex <= 0) continue;

                    string colName   = pkRow["name"]  as string ?? string.Empty;
                    string colType   = pkRow["type"]  as string ?? string.Empty;

                    bool hasExplicitAutoIncrement = tableDdl.IndexOf("AUTOINCREMENT",
                        StringComparison.OrdinalIgnoreCase) >= 0;

                    // Form 1: single-column INTEGER PRIMARY KEY (rowid alias)
                    bool isRowidAlias = pkColCount == 1 &&
                        string.Equals(colType.Trim(), "INTEGER", StringComparison.OrdinalIgnoreCase);

                    if (hasExplicitAutoIncrement || isRowidAlias)
                    {
                        SQLiteColumn col = this[colName] as SQLiteColumn;
                        if (col != null)
                        {
                            col._isAutoKey  = true;
                            col._autoInc    = 1; // SQLite auto-increment seed is always 1
                            col._autoSeed   = 1; // SQLite auto-increment step is always 1
                        }
                    }
                }

                cn.Close();
            }
            catch
            {
                if (cn != null && cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        override internal void LoadForView()
		{
			try
			{
				string query = 	"select * from information_schema.columns where table_catalog = '" + 
					this.View.Database.Name + "' and table_schema = '" + this.View.Schema + 
					"' and table_name = '" + this.View.Name + "' order by ordinal_position";

				IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.View.Database.Name);

				DataTable metaData = new DataTable();
                DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, cn);

				adapter.Fill(metaData);
				cn.Close();

				metaData.Columns["udt_name"].ColumnName  = "TYPE_NAME";
				metaData.Columns["data_type"].ColumnName = "TYPE_NAMECOMPLETE";

				if(metaData.Columns.Contains("TYPE_NAME"))
					f_TypeName = metaData.Columns["TYPE_NAME"];

				if(metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
					f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];
		
				PopulateArray(metaData);
			}
			catch {}
		}
	}
}

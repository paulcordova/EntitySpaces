using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteTables : Tables
	{
		public SQLiteTables()
		{

		}

		override internal void LoadAll()
		{
			try
            {
                // SQLite has no information_schema — use sqlite_master instead.
                // Filter type='table' to exclude views, triggers, and indexes.
                // Exclude internal sqlite_* tables (e.g. sqlite_sequence) to avoid
                // exposing SQLite internals in the Studio table list.
                string query = "SELECT name AS TABLE_NAME, '' AS TABLE_SCHEMA " +
                               "FROM sqlite_master " +
                               "WHERE type = 'table' AND name NOT LIKE 'sqlite_%' " +
                               "ORDER BY name;";

				IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

				DataTable metaData = new DataTable();
                DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, cn);

				adapter.Fill(metaData);
				cn.Close();
		
				PopulateArray(metaData);
			}
			catch {}
		}
	}
}

using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteViews : Views
	{
		public SQLiteViews()
		{

		}

		override internal void LoadAll()
		{
			try
			{
                // ============================================================
                // CHANGE: Use sqlite_master instead of information_schema.views
                // ============================================================
                string query = "SELECT name AS TABLE_NAME, '' AS TABLE_SCHEMA FROM sqlite_master WHERE type='view' ORDER BY name;";
                // ============================================================

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

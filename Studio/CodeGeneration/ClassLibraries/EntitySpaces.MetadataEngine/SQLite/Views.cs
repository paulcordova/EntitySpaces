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
				string query = "select * from information_schema.views where table_schema = '" +  this.Database.SchemaName + "'";

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

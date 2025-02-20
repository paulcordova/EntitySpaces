using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdViews : Views
	{
		public FirebirdViews()
		{

		}

		override internal void LoadAll()
		{
			try
			{

				//string query = @"SELECT r.*
				string query = @"SELECT TRIM(r.RDB$RELATION_NAME) AS table_name
								FROM RDB$RELATIONS r
								WHERE r.RDB$VIEW_BLR IS NOT NULL
								AND (r.RDB$SYSTEM_FLAG IS NULL OR r.RDB$SYSTEM_FLAG = 0)
								ORDER BY r.RDB$RELATION_NAME;";


				IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

				DataTable metaData = new DataTable();
                DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

				adapter.Fill(metaData);
				cn.Close();
		
				PopulateArray(metaData);
			}
			catch {}
		}
	}
}

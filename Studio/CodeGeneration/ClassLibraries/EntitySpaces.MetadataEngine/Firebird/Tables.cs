using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdTables : Tables
	{
		public FirebirdTables()
		{

		}

		override internal void LoadAll()
		{
			try
            {
                string query = @"SELECT trim(r.RDB$RELATION_NAME) AS table_name
								 FROM RDB$RELATIONS r
								 WHERE r.RDB$SYSTEM_FLAG = 0
								 AND r.RDB$VIEW_SOURCE IS NULL
								 ORDER BY r.RDB$RELATION_NAME;";


                IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);
							
				DataTable metaData = new DataTable();
				DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, this.dbRoot.ConnectionString);


				adapter.Fill(metaData);
				cn.Close();
		
				PopulateArray(metaData);
			}
			catch {}
		}
	}
}

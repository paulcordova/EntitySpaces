using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteDomains : Domains
	{
		public SQLiteDomains()
		{

		}

		internal DataColumn f_TypeNameComplete	= null;

		internal override void LoadAll()
		{
            string query = "SELECT name AS OBJECT_NAME, type AS OBJECT_TYPE FROM sqlite_master WHERE type IN ('table', 'view') ORDER BY name;";


            IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

			DataTable metaData = new DataTable();
            DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, cn);

			adapter.Fill(metaData);
			cn.Close();

			metaData.Columns["udt_name"].ColumnName = "DATA_TYPE";
			metaData.Columns["data_type"].ColumnName = "TYPE_NAMECOMPLETE";

			if(metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
				f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];
		
			PopulateArray(metaData);
		}
	}
}

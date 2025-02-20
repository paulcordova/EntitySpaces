using System;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdDomains : Domains
	{
		public FirebirdDomains()
		{

		}

		internal DataColumn f_TypeNameComplete	= null;

		internal override void LoadAll()
		{
			string query = @"SELECT 
								TRIM(f.RDB$FIELD_NAME) AS domain_name,
								TRIM(f.RDB$FIELD_TYPE) AS data_type,
								TRIM(f.RDB$FIELD_SUB_TYPE) AS sub_type,
								TRIM(f.RDB$FIELD_LENGTH) AS field_length,
								TRIM(f.RDB$NULL_FLAG) AS is_not_null,
								TRIM(f.RDB$DEFAULT_SOURCE) AS default_value,
								TRIM(f.RDB$VALIDATION_SOURCE) AS check_constraint,
								TRIM(f.RDB$DESCRIPTION) AS domain_description
				FROM RDB$FIELDS f
				WHERE f.RDB$SYSTEM_FLAG = 0"; 
				

			IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

			DataTable metaData = new DataTable();
            DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

			adapter.Fill(metaData);
			cn.Close();
		
			PopulateArray(metaData);
		}
	}
}

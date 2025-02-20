using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdProcedure : Procedure
	{
		internal string _specific_name = "";

		public FirebirdProcedure()
		{

		}

		public override IParameters Parameters
		{
			get
			{
				if(null == _parameters)
				{
					_parameters = (FirebirdParameters)this.dbRoot.ClassFactory.CreateParameters();
					_parameters.Procedure = this;
					_parameters.dbRoot = this.dbRoot;

					string query = "select * from information_schema.parameters where specific_schema = '" +
						this.Database.SchemaName + "' and specific_name = '" + (string)this._row["specific_name"] + "'";

					IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

					DataTable metaData = new DataTable();
                    DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

					adapter.Fill(metaData);
					cn.Close();

					metaData.Columns["udt_name"].ColumnName = "TYPE_NAME";
					metaData.Columns["data_type"].ColumnName = "TYPE_NAMECOMPLETE";

					if(metaData.Columns.Contains("TYPE_NAME"))
						_parameters.f_TypeName = metaData.Columns["TYPE_NAME"];

					if(metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
						_parameters.f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];
		
					_parameters.PopulateArray(metaData);

				}
				return _parameters;
			}
		}

		private FirebirdParameters _parameters = null;
	}
}

using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdTable : Table
	{
		public FirebirdTable()
		{

		}


		public override IColumns PrimaryKeys
		{
			get
			{
				if(null == _primaryKeys)
				{
					string query = "SELECT s.RDB$FIELD_NAME AS column_name " +
									"FROM RDB$RELATION_CONSTRAINTS rc " +
									"JOIN RDB$INDEX_SEGMENTS s ON rc.RDB$INDEX_NAME = s.RDB$INDEX_NAME " +
									"WHERE rc.RDB$RELATION_NAME = '" + this.Name.Trim() + "' " +
									"AND rc.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY' " +
									"ORDER BY s.RDB$FIELD_POSITION;";

					IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

					DataTable metaData = new DataTable();
                    DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

					adapter.Fill(metaData);
					cn.Close();

					_primaryKeys = (Columns)this.dbRoot.ClassFactory.CreateColumns();
					_primaryKeys.Table = this;
					_primaryKeys.dbRoot = this.dbRoot;

					string colName = "";

					int count = metaData.Rows.Count;
					for(int i = 0; i < count; i++)
					{
						colName = metaData.Rows[i]["COLUMN_NAME"] as string;
						_primaryKeys.AddColumn((Column)this.Columns[colName.Trim()]);
					}
				}

				return _primaryKeys;
			}
		}
	}
}

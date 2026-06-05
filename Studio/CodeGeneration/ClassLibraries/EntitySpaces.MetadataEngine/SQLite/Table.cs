using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteTable : Table
	{
		public SQLiteTable()
		{

		}


		public override IColumns PrimaryKeys
		{
			get
			{
				if(null == _primaryKeys)
				{
                    // SQLite has no information_schema.key_column_usage.
                    // PRAGMA table_info is the authoritative source for PK columns:
                    // the "pk" column holds the 1-based position of the column within
                    // the primary key (0 = not part of PK, 1+ = PK member).
                    string query = $"PRAGMA table_info(\"{this.Name}\")";

					IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Database.Name);

					DataTable metaData = new DataTable();
                    DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, cn);

					adapter.Fill(metaData);
					cn.Close();

					_primaryKeys = (Columns)this.dbRoot.ClassFactory.CreateColumns();
					_primaryKeys.Table = this;
					_primaryKeys.dbRoot = this.dbRoot;

					foreach (DataRow row in metaData.Rows)
					{
                        int pkIndex = row["pk"] != DBNull.Value ? Convert.ToInt32(row["pk"]) : 0;
                        if (pkIndex > 0)
                        {
                            string colName = row["name"] as string;
                            _primaryKeys.AddColumn((Column)this.Columns[colName]);
                        }
					}
				}

				return _primaryKeys;
			}
		}
	}
}

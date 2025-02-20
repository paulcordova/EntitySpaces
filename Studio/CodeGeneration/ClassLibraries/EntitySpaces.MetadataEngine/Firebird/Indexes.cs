using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdIndexes : Indexes
	{
		public FirebirdIndexes()
		{

		}

		override internal void LoadAll()
		{
			try
			{

                string select = @"SELECT 
                    trim(r.RDB$RELATION_NAME) AS table_name,
                    trim(i.RDB$INDEX_NAME) AS index_name,
                    i.RDB$UNIQUE_FLAG AS is_unique,
                    i.RDB$INDEX_INACTIVE AS is_inactive,
                    trim(s.RDB$FIELD_NAME) AS column_name
                FROM RDB$INDICES i
                JOIN RDB$RELATIONS r ON i.RDB$RELATION_NAME = r.RDB$RELATION_NAME
                JOIN RDB$INDEX_SEGMENTS s ON i.RDB$INDEX_NAME = s.RDB$INDEX_NAME
                WHERE r.RDB$RELATION_NAME = '" + this.Table.Name + @"'
                ORDER BY i.RDB$INDEX_NAME, s.RDB$FIELD_POSITION;";

                IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Name);

                DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(select, cn);
				DataTable metaData = new DataTable();

				adapter.Fill(metaData);
				cn.Close();
		
				PopulateArrayNoHookup(metaData);

				for(int i = 0; i < this.Count; i++)
				{
					Index index = this[i] as Index;

					if(null != index)
					{
						string s = index._row["columns"] as string;
						string[] colIndexes = s.Split(' ');

						foreach(string colIndex in colIndexes)
						{
							if(colIndex != "0")
							{
								int id = Convert.ToInt32(colIndex);

								Column column  = this.Table.Columns[id-1] as Column;
								index.AddColumn(column.Name);
							}
						}
					}
				}
			}
			catch {}
		}
	}
}

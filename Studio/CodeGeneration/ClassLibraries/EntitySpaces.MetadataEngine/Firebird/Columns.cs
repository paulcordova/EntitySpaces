using System;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdColumns : Columns
	{
		public FirebirdColumns()
		{

		}

		internal DataColumn f_TypeName = null;
		internal DataColumn f_TypeNameComplete	= null;

		override internal void LoadForTable()
		{
			IDbConnection cn = null;

			try
			{
                // Query to retrieve column information for the specified table in Firebird
                string query = @"SELECT " +
                    "TRIM(rf.RDB$FIELD_NAME) AS COLUMN_NAME, " +
                    "TRIM(t.rdb$type_name) as TYPE_NAME, " +
                    "TRIM(t.rdb$type_name) as TYPE_NAMECOMPLETE, " +
                    "TRIM(f.RDB$CHARACTER_LENGTH) AS DATA_LENGTH, " +
                    "CASE " +
                        "WHEN rf.RDB$NULL_FLAG IS NULL THEN '1' " +
                        "ELSE '0' " +
                    "END AS IS_NULLABLE, " +
                    "rf.RDB$DEFAULT_SOURCE AS COLUMN_DEFAULT, " +
                    "rf.RDB$FIELD_POSITION + 1 AS ORDINAL_POSITION " +
                    "FROM RDB$RELATION_FIELDS rf " +
                    "JOIN RDB$FIELDS f ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME " +
                    "join RDB$TYPES t on f.rdb$field_type = t.rdb$type " +
                    "WHERE " +
                    "rf.RDB$RELATION_NAME = '" + this.Table.FullName.Trim() + "' " +
                    " and t.rdb$field_name = 'RDB$FIELD_TYPE' " +
                    "ORDER BY " +
                    "rf.RDB$FIELD_POSITION";

                cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name);

				DataTable metaData = new DataTable();
                DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

				adapter.Fill(metaData);
				
				if(metaData.Columns.Contains("TYPE_NAME"))
					f_TypeName = metaData.Columns["TYPE_NAME"];

				if(metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
					f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];
		
				PopulateArray(metaData);

                // IsAutoKey logic
				query = @"SELECT " +
							"TRIM(r.RDB$FIELD_NAME) AS column_name, " +
                            "TRIM(r.RDB$FIELD_SOURCE) AS field_source, " +
                            "TRIM(r.rdb$generator_name) as generator_name, " +
                            "TRIM(f.RDB$FIELD_TYPE) AS data_type, " +
                            "TRIM(f.RDB$FIELD_SUB_TYPE) AS sub_type, " +
                            "TRIM(f.RDB$FIELD_LENGTH) AS field_length, " +
                            "TRIM(f.RDB$NULL_FLAG) AS is_not_null, " +
                            "TRIM(r.RDB$DEFAULT_SOURCE) AS default_value, " +
                            "TRIM(r.RDB$DESCRIPTION) AS column_description " +
						"FROM RDB$RELATION_FIELDS r " +
						"JOIN RDB$FIELDS f ON r.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME " +
						"WHERE r.RDB$RELATION_NAME = '" + this.Table.Name.Trim() + "'" +
                        "     AND   r.RDB$IDENTITY_TYPE = 1" +
						"ORDER BY r.RDB$FIELD_POSITION;";


                DataTable seqData = new DataTable();
                adapter = FirebirdDatabases.CreateAdapter(query, cn);
				adapter.Fill(seqData);

				DataRowCollection rows = seqData.Rows;

				if(rows.Count > 0)
				{
					string colName;

					for(int i = 0; i < rows.Count; i++)
					{
						colName = rows[i]["column_name"] as string;
						string colSource = rows[i]["generator_name"] as string;

						FirebirdColumn col = this[i] as FirebirdColumn;
						col._isAutoKey = true;

                        // Handle Firebird metadata for auto-increment columns
                        if (!string.IsNullOrEmpty(col.Default))
                        {
                            if (col.Default.ToUpper().Contains("NEXT VALUE FOR"))
                            {
                                col.AutoKeyText = col.Default.ToUpper().Replace("NEXT VALUE FOR", "").Trim();
                            }
                            else if (col.Default.ToUpper().Contains("GEN_ID"))
                            {
                                // Extract generator name from GEN_ID(generator_name, step)
                                int start = col.Default.IndexOf("(");
                                int end = col.Default.IndexOf(",");
                                if (start > 0 && end > start)
                                {
                                    col.AutoKeyText = col.Default.Substring(start + 1, end - start - 1).Trim();
                                }
                            }
                        }

                        query = "SELECT trim(RDB$GENERATOR_NAME) AS SEQUENCE_NAME, " +
                                 " RDB$INITIAL_VALUE as min_value, " +
                                 " RDB$GENERATOR_INCREMENT as increment_by " +
								 "FROM RDB$GENERATORS " +
								 "WHERE RDB$GENERATOR_NAME = '" + colSource + "'";

                        adapter = FirebirdDatabases.CreateAdapter(query, cn);
						DataTable autokeyData = new DataTable();
						adapter.Fill(autokeyData);

						Int64 a;
						Int32 b;
						
						if (autokeyData.Rows.Count > 0)
						{
							a = Convert.ToInt64(autokeyData.Rows[0]["min_value"]);
							b = Convert.ToInt32(autokeyData.Rows[0]["increment_by"]);
						}
					}
				}

				cn.Close();
			}
			catch
			{
				if(cn != null)
				{
					if(cn.State == ConnectionState.Open)
					{
						cn.Close();
					}
				}
			}
		}

		override internal void LoadForView()
		{
			try
			{

                //string query = @"SELECT 
                //					TRIM(rf.RDB$FIELD_NAME) AS column_name,
                //					TRIM(f.RDB$FIELD_TYPE) AS data_type,
                //					TRIM(f.RDB$FIELD_SUB_TYPE) AS sub_type,
                //					TRIM(f.RDB$FIELD_LENGTH) AS field_length,
                //					TRIM(f.RDB$NULL_FLAG) AS is_not_null,
                //					TRIM(r.RDB$DEFAULT_SOURCE) AS default_value,
                //					TRIM(r.RDB$DESCRIPTION) AS column_description
                //				FROM RDB$RELATION_FIELDS rf
                //				JOIN RDB$FIELDS f ON r.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME
                //				JOIN RDB$RELATIONS rel ON r.RDB$RELATION_NAME = rel.RDB$RELATION_NAME
                //				join RDB$TYPES t on f.rdb$field_type = t.rdb$type
                //				WHERE rel.RDB$VIEW_SOURCE IS NOT NULL
                //				AND r.RDB$RELATION_NAME = '" + this.View.Name.Trim() + @"'
                //				AND t.rdb$field_name = 'RDB$FIELD_TYPE'
                //				ORDER BY r.RDB$FIELD_POSITION;";

                string query = @"SELECT " +
                   "TRIM(rf.RDB$FIELD_NAME) AS COLUMN_NAME, " +
                   "TRIM(t.rdb$type_name) as TYPE_NAME, " +
                   "TRIM(t.rdb$type_name) as TYPE_NAMECOMPLETE, " +
                   "TRIM(f.RDB$CHARACTER_LENGTH) AS DATA_LENGTH, " +
                   "CASE " +
                       "WHEN rf.RDB$NULL_FLAG IS NULL THEN '1' " +
                       "ELSE '0' " +
                   "END AS IS_NULLABLE, " +
                   "rf.RDB$DEFAULT_SOURCE AS COLUMN_DEFAULT, " +
                   "rf.RDB$FIELD_POSITION + 1 AS ORDINAL_POSITION " +
                   "FROM RDB$RELATION_FIELDS rf " +
                   "JOIN RDB$FIELDS f ON rf.RDB$FIELD_SOURCE = f.RDB$FIELD_NAME " +
                   "join RDB$TYPES t on f.rdb$field_type = t.rdb$type " +
                   "WHERE " +
                   "rf.RDB$RELATION_NAME = '" + this.View.Name.Trim() + "' " +
                   " and t.rdb$field_name = 'RDB$FIELD_TYPE' " +
                   "ORDER BY " +
                   "rf.RDB$FIELD_POSITION";


                IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.View.Database.Name);

				DataTable metaData = new DataTable();
                DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query, cn);

				adapter.Fill(metaData);
				cn.Close();

				//metaData.Columns["COLUMN_NAME"].ColumnName  = "TYPE_NAME";
				//metaData.Columns["TYPE_NAMECOMPLETE"].ColumnName = "TYPE_NAMECOMPLETE";

				if(metaData.Columns.Contains("TYPE_NAME"))
					f_TypeName = metaData.Columns["TYPE_NAME"];

				if(metaData.Columns.Contains("TYPE_NAMECOMPLETE"))
					f_TypeNameComplete = metaData.Columns["TYPE_NAMECOMPLETE"];
		
				PopulateArray(metaData);
			}
			catch {}
		}
	}
}

using System;
using System.Data;
using System.Data.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdForeignKeys : ForeignKeys
	{
      
        static string _query =
                                "SELECT trim(rc.RDB$CONSTRAINT_NAME) AS FK_NAME, " +
                                       "trim(pp.RDB$RELATION_NAME) AS PK_TABLE_NAME, " +
                                       "trim(pf.RDB$FIELD_NAME) AS PK_COLS, " +
                                       "trim(rf.RDB$RELATION_NAME) AS FK_TABLE_NAME, " +
                                       "trim(ff.RDB$FIELD_NAME) AS FK_COLS " +
                                      // "rc.RDB$UPDATE_RULE AS UPDATE_RULE, " +
                                      // "rc.RDB$DELETE_RULE AS DELETE_RULE " +
                                "FROM RDB$RELATION_CONSTRAINTS rc " +
                                "JOIN RDB$REF_CONSTRAINTS ref ON rc.RDB$CONSTRAINT_NAME = ref.RDB$CONSTRAINT_NAME " +
                                "JOIN RDB$RELATION_CONSTRAINTS pp ON ref.RDB$CONST_NAME_UQ = pp.RDB$CONSTRAINT_NAME " +
                                "JOIN RDB$INDEX_SEGMENTS pf ON pp.RDB$INDEX_NAME = pf.RDB$INDEX_NAME " +
                                "JOIN RDB$RELATION_CONSTRAINTS rf ON rc.RDB$CONSTRAINT_NAME = rf.RDB$CONSTRAINT_NAME " +
                                "JOIN RDB$INDEX_SEGMENTS ff ON rf.RDB$INDEX_NAME = ff.RDB$INDEX_NAME " +
                                "WHERE rc.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' ";
                                

        public FirebirdForeignKeys()
		{

		}

		override internal void LoadAll()
		{
            string query1 = _query +
                        " AND pp.RDB$RELATION_NAME = '" + this.Table.Name.Trim() + "' " +
                        " ORDER BY rc.RDB$CONSTRAINT_NAME";

            string query2 = _query +
                        " AND rf.RDB$RELATION_NAME = '" + this.Table.Name.Trim() + "' " +
                        " ORDER BY rc.RDB$CONSTRAINT_NAME";


            this._LoadAll(query1, query2);
		}

		private void _LoadAll(string query1, string query2)
		{
			IDbConnection cn = null;

            try
            {
                cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name);

                DataTable metaData1 = new DataTable();
                DataTable metaData2 = new DataTable();

                DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(query1, cn);
                adapter.Fill(metaData1);

                adapter = FirebirdDatabases.CreateAdapter(query2, cn);
                adapter.Fill(metaData2);

                DataRowCollection rows = metaData2.Rows;
                int count = rows.Count;
                for (int i = 0; i < count; i++)
                {
                    metaData1.ImportRow(rows[i]);
                }

                PopulateArrayNoHookup(metaData1);

                if (metaData1.Rows.Count > 0)
                {
                    string catalog = ""; // this.Table.Database.Name;
                    string schema;
                    string table;
                    string[] cols = null;
                    string q;

                    string query =
                            "SELECT " +
                                    "trim(fk.RDB$CONSTRAINT_NAME) AS constraint_name, " +
                                    "trim(fk.RDB$RELATION_NAME) AS table_name, " +        //Table containing the FK
                                    "trim(fk_seg.RDB$FIELD_NAME) AS constraint_key, " +  //FK column in parent table
                                    "trim(pk.RDB$RELATION_NAME) AS references_table, " + //Referenced table (PK)
                                    "trim(pk_seg.RDB$FIELD_NAME) AS fk_constraint_key " + //Column referenced in the PK
                                "FROM RDB$REF_CONSTRAINTS ref " +
                                "JOIN RDB$RELATION_CONSTRAINTS fk ON fk.RDB$CONSTRAINT_NAME = ref.RDB$CONSTRAINT_NAME " +
                                "JOIN RDB$RELATION_CONSTRAINTS pk ON pk.RDB$CONSTRAINT_NAME = ref.RDB$CONST_NAME_UQ " +
                                "JOIN RDB$INDEX_SEGMENTS fk_seg ON fk.RDB$INDEX_NAME = fk_seg.RDB$INDEX_NAME " +
                                "JOIN RDB$INDEX_SEGMENTS pk_seg ON pk.RDB$INDEX_NAME = pk_seg.RDB$INDEX_NAME " +
                                "WHERE fk.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' " +
                                "AND fk.RDB$CONSTRAINT_NAME = ";


                    foreach (ForeignKey key in this)
                    {
                        //------------------------------------------------
                        // Primary
                        //------------------------------------------------
                        schema = ""; // Firebird don't has Schemma
                        table  = key._row["PK_TABLE_NAME"] as string;

                        string keyName = string.Empty;

                        try
                        {
                            keyName = key.Name.Split('.')[1];
                        }
                        catch
                        {
                            keyName = key.Name.Trim();
                        }

                        q = query;
                        q += "'" + keyName + "'";

                        DataTable metaData = new DataTable();
                        adapter = FirebirdDatabases.CreateAdapter(q, cn);

                        adapter.Fill(metaData);

                        string[] ordinals = ((string)metaData.Rows[0][4]).Split(' ');

                        foreach (string ordinal in ordinals)
                        {
                            int c = key.PrimaryTable.Columns.Count;
                            string colName = ordinal;
                            key.AddForeignColumn(catalog.Trim(), "", table.Trim(), colName.Trim(), true);
                        }

                        //------------------------------------------------
                        // Foreign
                        //------------------------------------------------
                        schema = ""; //Firebird don't has Schemma
                        table  = key._row["FK_TABLE_NAME"] as string;

                        ordinals = ((string)metaData.Rows[0][2]).Split(' ');

                        foreach (string ordinal in ordinals)
                        {
                            int c = key.ForeignTable.Columns.Count;
                            // string colName = key.ForeignTable.Columns[Convert.ToInt32(ordinal) - 1].Name;
                            string colName = ordinal;
                            key.AddForeignColumn(catalog, "", table.Trim(), colName.Trim(), false);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }

			cn.Close();
		}

		private string[] ParseColumns(string cols)
		{
			cols = cols.Replace("{", "");
			cols = cols.Replace("}", "");
			return cols.Split(',');
		}
	}
}

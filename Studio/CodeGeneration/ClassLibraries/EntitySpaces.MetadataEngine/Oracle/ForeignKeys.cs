using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleForeignKeys : ForeignKeys
    {
        public OracleForeignKeys()
        {
        }

        override internal void LoadAll()
        {
            try
            {
                string query =
                    "SELECT " +
                    "  ''                                    AS PK_TABLE_CATALOG, " +
                    "  pk.OWNER                              AS PK_TABLE_SCHEMA, " +
                    "  pk.TABLE_NAME                         AS PK_TABLE_NAME, " +
                    "  pkc.COLUMN_NAME                       AS PK_COLUMN_NAME, " +
                    "  ''                                    AS FK_TABLE_CATALOG, " +
                    "  fk.OWNER                              AS FK_TABLE_SCHEMA, " +
                    "  fk.TABLE_NAME                         AS FK_TABLE_NAME, " +
                    "  fkc.COLUMN_NAME                       AS FK_COLUMN_NAME, " +
                    "  pk.CONSTRAINT_NAME                    AS PK_NAME, " +
                    "  fk.CONSTRAINT_NAME                    AS FK_NAME, " +
                    "  pkc.POSITION                          AS ORDINAL, " +
                    "  'NO ACTION'                           AS UPDATE_RULE, " +
                    "  fk.DELETE_RULE                        AS DELETE_RULE " +
                    "FROM ALL_CONSTRAINTS fk " +
                    "JOIN ALL_CONSTRAINTS pk " +
                    "  ON pk.CONSTRAINT_NAME = fk.R_CONSTRAINT_NAME " +
                    " AND pk.OWNER           = fk.R_OWNER " +
                    "JOIN ALL_CONS_COLUMNS pkc " +
                    "  ON pkc.CONSTRAINT_NAME = pk.CONSTRAINT_NAME " +
                    " AND pkc.OWNER           = pk.OWNER " +
                    "JOIN ALL_CONS_COLUMNS fkc " +
                    "  ON fkc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME " +
                    " AND fkc.OWNER           = fk.OWNER " +
                    " AND fkc.POSITION        = pkc.POSITION " +
                    "WHERE fk.CONSTRAINT_TYPE = 'R' " +
                    "  AND ( " +
                    "    (pk.OWNER = '" + this.Table.Database.SchemaOwner + "' AND pk.TABLE_NAME = '" + this.Table.Name + "') " +
                    "    OR " +
                    "    (fk.OWNER = '" + this.Table.Database.SchemaOwner + "' AND fk.TABLE_NAME = '" + this.Table.Name + "') " +
                    "  ) " +
                    "ORDER BY fk.CONSTRAINT_NAME, pkc.POSITION";

                DataTable metaData = new DataTable();

                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();
                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                PopulateArray(metaData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
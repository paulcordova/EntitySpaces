using System;
using System.Data;
using OracleClient = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleTables : Tables
    {
        public OracleTables()
        {
        }

        override internal void LoadAll()
        {
            try
            {
                string query = @"SELECT OWNER, table_name" +
                                " FROM all_tables" +
                                " WHERE owner = :schema" +
                                " ORDER BY table_name";

                DataTable metaData = new DataTable();

                using (OracleClient.OracleConnection cn = new OracleClient.OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleClient.OracleCommand cmd = new OracleClient.OracleCommand(query, cn))
                    {
                        cmd.BindByName = true; // Crucial para que ODP.NET enlace correctamente
                        cmd.Parameters.Add(new OracleClient.OracleParameter("schema", this.Database.SchemaOwner));

                        using (OracleClient.OracleDataAdapter adapter = new OracleClient.OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                PopulateArray(metaData);
                LoadExtraData(this.Database.SchemaOwner);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void LoadExtraData(string schema)
        {
            try
            {
                string select = "SELECT DISTINCT C.TABLE_NAME, C.COMMENTS AS DESCRIPTION" + 
                                " FROM SYS.ALL_TAB_COMMENTS C, SYS.ALL_TABLES T" +
                                " WHERE T.OWNER = :schema AND" +
                                " T.OWNER = C.OWNER AND" + 
                                " T.TABLE_NAME = C.TABLE_NAME AND" +
                                " C.COMMENTS IS NOT NULL";

                DataTable metaData = new DataTable();

                using (OracleClient.OracleConnection cn = new OracleClient.OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleClient.OracleCommand cmd = new OracleClient.OracleCommand(select, cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new OracleClient.OracleParameter("schema", schema));

                        using (OracleClient.OracleDataAdapter adapter = new OracleClient.OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                DataRowCollection rows = metaData.Rows;

                if (rows.Count > 0)
                {
                    Table t;
                    foreach (DataRow row in rows)
                    {
                        t = this[row["TABLE_NAME"]] as Table;
                        if (t != null)
                        {
                            t._row["DESCRIPTION"] = row["DESCRIPTION"];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
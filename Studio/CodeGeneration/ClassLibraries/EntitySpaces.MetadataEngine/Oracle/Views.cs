using System;
using System.Data;
using System.IO; // Required for logging
using Oracle.ManagedDataAccess.Client;
using OdpNet = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleViews : Views
    {
        public OracleViews()
        {
        }

        internal override void LoadAll()
        {
            try
            {
                // We use aliases (AS TABLE_NAME, AS TABLE_SCHEMA) to match what 
                // the MetadataEngine base class expects for View objects.
                string query = @"SELECT 
                            VIEW_NAME AS TABLE_NAME, 
                            OWNER AS TABLE_SCHEMA,
                            'VIEW' AS TABLE_TYPE
                         FROM ALL_VIEWS 
                         WHERE OWNER = :owner";

                DataTable metaData = new DataTable();

                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(new global::Oracle.ManagedDataAccess.Client.OracleParameter("owner", this.Database.SchemaOwner));

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(metaData);
                        }
                    }
                }

                // Now PopulateArray will find the TABLE_NAME and TABLE_SCHEMA columns it needs
                base.PopulateArray(metaData);
            }
            catch (Exception ex)
            {
                // Debugging log if something fails
                string logPath = @"C:\oracle\view_debug.txt";
                using (StreamWriter sw = new StreamWriter(logPath, true))
                {
                    sw.WriteLine("EXCEPTION: " + ex.Message);
                }
                throw;
            }
        }
    }
}
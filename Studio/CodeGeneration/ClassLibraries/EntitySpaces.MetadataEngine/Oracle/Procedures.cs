using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using OdpNet = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleProcedures : Procedures
	{
		public OracleProcedures()
		{

		}

     
        override internal void LoadAll()
        {
            try
            {
                DataTable metaData = new DataTable();

                string query = "SELECT * FROM ALL_PROCEDURES WHERE " +
                                " OBJECT_OWNER = :databaseOwner";

                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        cmd.BindByName = true;

                        // Use full ODP.NET type to avoid collision with local OracleParameter class.
                        // OracleParameter(string name, object value) — sets both bind name and value.
                        cmd.Parameters.Add(new OdpNet.OracleParameter("databaseOwner", this.Database.SchemaOwner));

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

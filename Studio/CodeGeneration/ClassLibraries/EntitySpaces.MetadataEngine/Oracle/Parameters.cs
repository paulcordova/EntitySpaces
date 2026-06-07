using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using OdpNet = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleParameters : Parameters
	{
		public OracleParameters()
		{

		}
       
        override internal void LoadAll()
        {
            try
            {
                DataTable metaData = new DataTable();

                string query = @"SELECT * 
                                FROM ALL_ARGUMENTS 
                                WHERE OBJECT_OWNER = :databaseOwner 
                                  AND OBJECT_NAME = :procedureName 
                                  AND OBJECT_TYPE = 'PROCEDURE'
                                ORDER BY SEQUENCE";

                using (OracleConnection cn = new OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();

                    using (OracleCommand cmd = new OracleCommand(query, cn))
                    {
                        cmd.BindByName = true;

                        // Use full ODP.NET type to avoid collision with local OracleParameter class.
                        // OracleParameter(string name, object value) — sets both bind name and value.
                        cmd.Parameters.Add(new OdpNet.OracleParameter("databaseOwner", this.Procedure.Database.Name));
                        cmd.Parameters.Add(new OdpNet.OracleParameter("procedureName", this.Procedure.Name));

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

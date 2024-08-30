using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	/// <summary>
	/// Summary description for ConnectionHelper.
	/// </summary>
	public class ConnectionHelper
	{
		public ConnectionHelper()
		{

		}

        static public IDbConnection CreateConnection(Root dbRoot, string database)
		{
            IDbConnection cn = new OracleConnection(dbRoot.ConnectionString);
			cn.Open();
			//cn.ChangeDatabase(database);
			return cn;
		}
	}
}

using System;
using System.Data;


namespace EntitySpaces.MetadataEngine.SQLite
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
            IDbConnection cn = SQLiteDatabases.CreateConnection(dbRoot.ConnectionString);
			cn.Open();
			cn.ChangeDatabase(database);
			return cn;
		}
	}
}

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
            // SQLite is a single-file database — there is no concept of "changing database"
            // at the connection level. ChangeDatabase() is not supported by System.Data.SQLite
            // and throws NotSupportedException. The connection string already points to the
            // correct file, so we simply open it and return.
            IDbConnection cn = SQLiteDatabases.CreateConnection(dbRoot.ConnectionString);
            cn.Open();
            return cn;
        }
	}
}

using System;
using System.Data;
using System.Data.OleDb;

using ADODB;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteDatabase : Database
	{
		public SQLiteDatabase()
		{

		}

		override public ADODB.Recordset ExecuteSql(string sql)
		{
            IDbConnection cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Name);

			return this.ExecuteIntoRecordset(sql, cn);
		}
	}
}

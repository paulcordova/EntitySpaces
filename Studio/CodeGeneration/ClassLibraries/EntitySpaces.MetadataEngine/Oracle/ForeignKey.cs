using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleForeignKey : ForeignKey
	{
		public OracleForeignKey()
		{

		}

		override public ITable ForeignTable
		{
			get
			{
				string catalog = this.ForeignKeys.Table.Database.Name;
				string schema  = this.GetString(ForeignKeys.f_FKTableSchema);

				return this.dbRoot.Databases[schema].Tables[this.GetString(ForeignKeys.f_FKTableName)];
			}
		}
	}
}

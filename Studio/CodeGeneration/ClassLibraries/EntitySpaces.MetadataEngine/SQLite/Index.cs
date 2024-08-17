using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteIndex : Index
	{
		public SQLiteIndex()
		{

		}

		public override string Type
		{
			get
			{
				string type = this.GetString(Indexes.f_Type);
				return type.ToUpper();
			}
		}
	}
}

using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdIndex : Index
	{
		public FirebirdIndex()
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

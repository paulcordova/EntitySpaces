using System;
using System.Data;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteDomain : Domain
	{
		public SQLiteDomain()
		{

		}

		public override string DataTypeNameComplete
		{
			get
			{
				SQLiteDomains domains = this.Domains as SQLiteDomains;
				return this.GetString(domains.f_TypeNameComplete);
			}
		}

	}
}

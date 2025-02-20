using System;
using System.Data;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdDomain : Domain
	{
		public FirebirdDomain()
		{

		}

		public override string DataTypeNameComplete
		{
			get
			{
				FirebirdDomains domains = this.Domains as FirebirdDomains;
				return this.GetString(domains.f_TypeNameComplete);
			}
		}

	}
}

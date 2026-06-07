using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
	public class OracleIndex : Index
	{
		public OracleIndex()
		{

		}

        public override string Type
        {
            get
            {
                string type = this.GetString(Indexes.f_Type);
                return type != null ? type.ToUpper() : string.Empty;
            }
        }
    }
}

using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Plugin
{
	public class PluginColumn : Column
	{
        private IPlugin plugin;

        public PluginColumn(IPlugin plugin)
        {
            this.plugin = plugin;
        }

		public override string DataTypeName
		{
			get
			{
				PluginColumns cols = Columns as PluginColumns;
				return this.GetString(cols.f_extTypeName);
			}
		}

		public override string DataTypeNameComplete
		{
			get
			{
				PluginColumns cols = Columns as PluginColumns;
				return this.GetString(cols.f_extTypeNameComplete);
			}
        }

        public override Boolean IsComputed
        {
            get
            {
                object val = this._row["IS_COMPUTED"];
                if (val == DBNull.Value) return false;
                return Convert.ToBoolean(val);
            }
        }

        public override object DatabaseSpecificMetaData(string key)
        {
            return this.plugin.GetDatabaseSpecificMetaData(this, key);
        }
	}
}

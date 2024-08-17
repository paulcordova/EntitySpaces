using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteParameter : Parameter
	{
		public SQLiteParameter()
		{

		}

		public override string Name
		{
			get
			{
				string n = base.Name;

				if(n == string.Empty)
				{
					n = "[" + this.Parameters.Procedure.Name + ":" + this.Ordinal.ToString() + "]";
				}

				return n;
			}
		}

		public override ParamDirection Direction
		{
			get
			{
				return ParamDirection.Input;
			}
		}



		override public string DataTypeNameComplete
		{
			get
			{
				SQLiteParameters parameters = this.Parameters as SQLiteParameters;
				return this.GetString(parameters.f_TypeNameComplete);
			}
		}
	}
}

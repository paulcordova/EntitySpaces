using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdParameter : Parameter
	{
		public FirebirdParameter()
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
				FirebirdParameters parameters = this.Parameters as FirebirdParameters;
				return this.GetString(parameters.f_TypeNameComplete);
			}
		}
	}
}

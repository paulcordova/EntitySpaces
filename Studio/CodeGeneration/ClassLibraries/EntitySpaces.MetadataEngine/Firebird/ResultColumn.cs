using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Firebird
{
	public class FirebirdResultColumn : ResultColumn
	{
		public FirebirdResultColumn()
		{

		}

		#region Properties

		override public string Name
		{
			get
			{
				return this._column.ColumnName;
			}
		}

		override public string DataTypeName
		{
			get
			{
				return _column.DataType.ToString();
			}
		}

		override public System.Int32 Ordinal
		{
			get
			{
				return this._column.Ordinal;
			}
		}

		#endregion

		internal DataColumn _column = null;
	}
}

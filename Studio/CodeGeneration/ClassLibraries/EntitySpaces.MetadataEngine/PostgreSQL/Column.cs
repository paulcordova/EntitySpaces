using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.PostgreSQL
{
	public class PostgreSQLColumn : Column
	{
		internal bool _isAutoKey = false;
		internal int _autoInc   = 0;
		internal int _autoSeed  = 0;

		public PostgreSQLColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

		override public System.Boolean IsNullable
		{
			get
			{
				string s = this.GetString(Columns.f_IsNullable);

				if(s == "YES") 
					return true;
				else
					return false;
			}
		}

		override public System.Boolean HasDefault
		{
			get
			{
				object o = this._row[Columns.f_Default];

				if(o == DBNull.Value)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}


		public override System.Boolean IsAutoKey
		{
			get
			{
				return this._isAutoKey;
			}
		}

		public override Int32 AutoKeyIncrement
		{
			get
			{
				return this._autoInc;
			}
		}

		public override Int32 AutoKeySeed
		{
			get
			{
				return this._autoSeed;
			}
		}

		override public string DataTypeName
		{
			get
			{
				PostgreSQLColumns cols = Columns as PostgreSQLColumns;
				return this.GetString(cols.f_TypeName);
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				PostgreSQLColumns cols = Columns as PostgreSQLColumns;
                return this.GetString(cols.f_TypeNameComplete).Replace("\'", string.Empty);
			}
		}

        // ============================================================
        // START: Metadata improvements (Precision, Scale, Length, Computed, Concurrency)
        // ============================================================

        public override Int32 CharacterMaxLength
        {
            get
            {
                object val = this._row["character_maximum_length"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Int32 NumericPrecision
        {
            get
            {
                object val = this._row["numeric_precision"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Int32 NumericScale
        {
            get
            {
                object val = this._row["numeric_scale"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Boolean IsComputed
        {
            get
            {
                object val = this._row["is_generated"];
                if (val == DBNull.Value) return false;
                string generated = val as string;
                return generated == "ALWAYS";
            }
        }

        public override Boolean IsConcurrency
        {
            get
            {
                string type = this.DataTypeName.ToLower();
                return type == "timestamp" || type == "timestamptz";
            }
        }

        // ============================================================
        // END: Metadata improvements
        // ============================================================

    }
}

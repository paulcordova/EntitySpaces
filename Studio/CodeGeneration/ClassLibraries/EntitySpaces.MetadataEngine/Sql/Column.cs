using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.Sql
{
	public class SqlColumn : Column
	{
		public SqlColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}
		
        public override bool IsConcurrency
        {
            get
            {
			    if(this.DataTypeName.ToLower() == "timestamp")
			    {
    				return true;
			    }

                return false;
            }
        }

        public override string LanguageType
        {
            get
            {
               return base.LanguageType;
            }
        }


		override public string DataTypeName
		{
			get
			{
				if(this.dbRoot.DomainOverride)
				{
					if(this.HasDomain)
					{
						if(this.Domain != null)
						{
							return this.Domain.DataTypeName;
						}
					}
				}

				SqlColumns cols = Columns as SqlColumns;
				return this.GetString(cols.f_TypeName);
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				if(this.dbRoot.DomainOverride)
				{
					if(this.HasDomain)
					{
						if(this.Domain != null)
						{
							return this.Domain.DataTypeNameComplete;
						}
					}
				}

                return GetFullDataTypeName(DataTypeName, CharacterMaxLength, NumericPrecision, NumericScale).Replace("\'", string.Empty);
			}
		}

		public override object DatabaseSpecificMetaData(string key)
		{
			return SqlDatabase.DBSpecific(key, this);
		}

        internal static string GetFullDataTypeName(string name, int charMaxLen, int precision, int scale)
        {
            string dtnf = null;
            switch (name)
            {
                case "varchar":
                case "nvarchar":
                case "varbinary":
                    if (charMaxLen > 1000000)
                        dtnf = name + "(MAX)";
                    else
                        dtnf = name + "(" + charMaxLen + ")";
                    break;
                case "binary":
                case "char":
                case "nchar":

                    dtnf = name + "(" + charMaxLen + ")";
                    break;

                case "decimal":
                case "numeric":

                    dtnf = name + "(" + precision + "," + scale + ")";
                    break;

                default:

                    dtnf = name;
                    break;
            }

            return dtnf;
        }


        // ============================================================
        // START: Metadata improvements (Length, Precision, Scale, Computed)
        // ============================================================

        /// <summary>
        /// Gets the maximum length for character columns (varchar, nvarchar, char, nchar).
        /// Reads from the OleDb schema column "CHARACTER_MAXIMUM_LENGTH".
        /// </summary>
        public override Int32 CharacterMaxLength
        {
            get
            {
                object val = this._row["CHARACTER_MAXIMUM_LENGTH"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        /// <summary>
        /// Gets the numeric precision for decimal/numeric columns.
        /// Reads from the OleDb schema column "NUMERIC_PRECISION".
        /// </summary>
        public override Int32 NumericPrecision
        {
            get
            {
                object val = this._row["NUMERIC_PRECISION"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        /// <summary>
        /// Gets the numeric scale for decimal/numeric columns.
        /// Reads from the OleDb schema column "NUMERIC_SCALE".
        /// </summary>
        public override Int32 NumericScale
        {
            get
            {
                object val = this._row["NUMERIC_SCALE"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        /// <summary>
        /// Indicates whether the column is a computed column (is_computed = 1 in sys.columns).
        /// Falls back to false if the value is not available.
        /// </summary>
        public override Boolean IsComputed
        {
            get
            {
                object val = this._row["IS_COMPUTED"];
                if (val != DBNull.Value)
                {
                    return Convert.ToBoolean(val);
                }
                return false;
            }
        }

        // ============================================================
        // END: Metadata improvements
        // ============================================================

    }
}

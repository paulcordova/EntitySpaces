using System;
using System.Data;

namespace EntitySpaces.MetadataEngine.MySql
{
	public class MySqlColumn : Column
	{
		static char[] chars = new char[] {' ', '('};

		private int numericScale = 0;
		private int precision = 0;
		private int characterLength = 0;
		private string dataType = "";

		public MySqlColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

		public override Boolean IsNullable
		{
			get
			{
				MySqlColumns cols = Columns as MySqlColumns;
				string s = this.GetString(cols.f_IsNullable);
				return (s == "YES") ? true : false;
			}
		}

		public override Boolean HasDefault
		{
			get
			{
				return (this.Default == "") ? false : true;
			}
		}

		override public string DataTypeName
		{
			get
			{
				if(dataType == "")
				{
					MySqlColumns cols = Columns as MySqlColumns;
					string type = this.GetString(cols.f_DataType).ToUpper();

					string[] data = type.Split(new char[]{' '});
					string[] typeandsize = data[0].Split(new char[]{'(',')',','});

					dataType = typeandsize[0];

					if(dataType != "ENUM")
					{
						if(-1 != type.IndexOf("UNSIGNED"))
						{
							dataType += " UNSIGNED";
						}

						int parts = typeandsize.Length;

						if(parts >= 2)
						{
							if(dataType == "VARCHAR" || dataType == "CHAR")
							{
								this.characterLength = Convert.ToInt32(typeandsize[1]);
							}
							else
							{
								this.precision = Convert.ToInt32(typeandsize[1]);
							}
						}

						if(parts >= 3)
						{
							if(typeandsize[2].Length > 0)
							{
								this.numericScale = Convert.ToInt32(typeandsize[2]);
							}
						}
					}
				}

				return dataType;
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
                string dataTypeNameComplete = "";

				try
				{
					MySqlColumns cols = Columns as MySqlColumns;
					string origType = GetString(cols.f_DataType);
					string type = origType.ToUpper();

					string[] data = type.Split(new char[]{' '});

					if(data[0].StartsWith("ENUM"))
					{
						dataTypeNameComplete = "ENUM" + origType.Substring(4, origType.Length - 4);
					}
					else
					{
						if(-1 != type.IndexOf("UNSIGNED"))
						{
							dataTypeNameComplete = data[0] + " UNSIGNED";
						}
						else
						{
							dataTypeNameComplete = data[0];
						}
					}
				}
				catch
				{
					dataTypeNameComplete = "ERROR";
				}

                return dataTypeNameComplete.Replace("\'", string.Empty);
			}
		}

		public override Int32 NumericPrecision
		{
			get
			{
				return this.precision;
			}
		}

		public override Int32 NumericScale
		{
			get
			{
				return this.numericScale;
			}
		}

		public override Int32 CharacterMaxLength
		{
			get
			{
				return this.characterLength;
			}
		}

        // ============================================================
        // START: Metadata improvements (Computed, Concurrency)
        // ============================================================

        /// <summary>
        /// Indicates whether the column is a generated column (GENERATED ALWAYS AS ...)
        /// Detected via the 'Extra' column from SHOW COLUMNS containing 'VIRTUAL GENERATED' or 'STORED GENERATED'.
        /// </summary>
        public override Boolean IsComputed
        {
            get
            {
                object val = this._row["IS_COMPUTED"];
                if (val == DBNull.Value) return false;
                return Convert.ToBoolean(val);
            }
        }

        /// <summary>
        /// Indicates whether the column is used for optimistic concurrency (e.g., TIMESTAMP)
        /// </summary>
        public override Boolean IsConcurrency
        {
            get
            {
                string type = this.DataTypeName.ToUpper();
                return type == "TIMESTAMP";
            }
        }

        /// <summary>
        /// EntitySpaces concurrency flag.
        ///
        /// Studio's Custom Fields -> Concurrency config is stored outside the DB
        /// and applied to the in-memory Column objects during BO generation, but
        /// not always during SP generation. The base Column.IsEntitySpacesConcurrency
        /// returns false in that path, which makes the SP template emit an IN
        /// parameter for the version column instead of OUT / INOUT.
        ///
        /// This override adds a name-based fallback on top of the base value so
        /// both generators see the same answer. The base value (when populated)
        /// always wins; we only turn false -> true when the column name matches
        /// the convention used by the DDL of this project.
        /// </summary>
        public override Boolean IsEntitySpacesConcurrency
        {
            get
            {
                // Base value first. Wrapped defensively because the base getter
                // may throw if the "IS_ENTITYSPACES_CONCURRENCY" DataColumn is not
                // present in _row (which is precisely the failure mode we are
                // working around).
                try
                {
                    if (base.IsEntitySpacesConcurrency) return true;
                }
                catch
                {
                    // fall through to the name-based fallback
                }

                string name = (this.Name ?? string.Empty).ToLowerInvariant();
                return name == "version";
            }
        }



        // ============================================================
        // END: Metadata improvements
        // ============================================================
    }
}

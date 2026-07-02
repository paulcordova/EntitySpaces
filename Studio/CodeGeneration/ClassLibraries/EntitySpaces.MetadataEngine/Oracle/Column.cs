using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleColumn : Column
    {

        internal bool _isAutoKey = false;
        internal int _autoInc = 0;
        internal int _autoSeed = 0;

        public OracleColumn()
        {

        }

        public override bool HasDefault
        {
            get
            {
                return (this.Default != null && this.Default.Length > 0);
            }
        }

        override internal Column Clone()
        {
            Column c = base.Clone();

            return c;
        }

        public override string DataTypeName
        {
            get
            {
                OracleColumns cols = Columns as OracleColumns;
                string rawType = this.GetString(cols.f_TypeName);

                if (string.IsNullOrEmpty(rawType))
                {
                    return "object";
                }

                string cleanType = rawType.Trim().ToUpper();

                // BINARY_FLOAT / BINARY_DOUBLE are Oracle native floating-point types.
                // esLanguages.xml has no entries for them — map to FLOAT which is defined
                // in the Oracle section and resolves to decimal/double correctly.
                if (cleanType == "BINARY_FLOAT" || cleanType == "BINARY_DOUBLE")
                {
                    Log: System.IO.File.AppendAllText(@"C:\oracle\bf_debug.txt",
                    this.Name + " | table=" + (Columns?.Table?.Name ?? "?") + "\n");
                    return "FLOAT";
                }

                // EntitySpaces base engine gets confused with "NUMBER" when it has a scale > 0,
                // causing it to look for aliases not present in esLanguages.xml and returning "Unknown".
                // By overriding it to "FLOAT", we force a direct, safe mapping to "decimal" 
                // since "FLOAT" is explicitly defined in the Oracle section of the XML.
                if (cleanType == "NUMBER" && this.NumericScale > 0)
                {
                    return "FLOAT";
                }

                return cleanType;
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


        override public string DataTypeNameComplete
        {
            get
            {
                return GetFullDataTypeName(DataTypeName, CharacterMaxLength, NumericPrecision, NumericScale).Replace("\'", string.Empty);
            }
        }

        internal static string GetFullDataTypeName(string name, int charMaxLen, int precision, int scale)
        {
            string dtnf = null;
            switch (name)
            {
                case "VARCHAR2":
                case "NVARCHAR2":
                case "RAW":
                case "LONGRAW":
                case "BFILE":
                case "BLOB":
                case "CHAR":
                case "NCHAR":
                    dtnf = name + "(" + charMaxLen + ")";
                    break;

                case "FLOAT":
                case "BINARY_FLOAT":
                case "BINARY_DOUBLE":
                    dtnf = precision > 0 ? "FLOAT(" + precision + ")" : "FLOAT";
                    break;

                case "INTEGER":
                    dtnf = name;
                    break;

                case "NUMBER":
                    if (precision > 0 && scale >= 0)
                    {
                        dtnf = name + "(" + precision + "," + scale + ")";
                    }
                    else if (precision > 0)
                    {
                        dtnf = name + "(" + precision + ")";
                    }
                    else
                    {
                        dtnf = name;
                    }
                    break;

                default:
                    dtnf = name;
                    break;
            }

            return dtnf;
        }

        // ============================================================
        // START: Metadata improvements (Length, Precision, Scale, Computed, Concurrency)
        // ============================================================

        public override Int32 CharacterMaxLength
        {
            get
            {
                object val = this._row["CHARACTER_MAXIMUM_LENGTH"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Int32 NumericPrecision
        {
            get
            {
                object val = this._row["NUMERIC_PRECISION"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Int32 NumericScale
        {
            get
            {
                object val = this._row["NUMERIC_SCALE"];
                if (val == DBNull.Value) return 0;
                return Convert.ToInt32(val);
            }
        }

        public override Boolean IsComputed
        {
            get
            {
                object val = this._row["IS_COMPUTED"];
                if (val == DBNull.Value) return false;
                string virtualCol = val as string;
                return virtualCol == "YES";
            }
        }

        public override Boolean IsConcurrency
        {
            get
            {
                string type = this.DataTypeName.ToUpper();
                return type == "TIMESTAMP";
            }
        }

        // ============================================================
        // END: Metadata improvements
        // ============================================================

    } //end class
} //end namespace

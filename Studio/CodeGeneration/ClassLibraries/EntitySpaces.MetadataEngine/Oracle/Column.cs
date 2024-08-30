using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

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
                return this.GetString(cols.f_TypeName);
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
                    dtnf = precision > 0 ? name + "(" + precision + ")" : name;
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

    }
}

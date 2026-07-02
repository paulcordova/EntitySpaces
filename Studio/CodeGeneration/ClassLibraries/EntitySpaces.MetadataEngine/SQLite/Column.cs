using System;
using System.Data;
using System.Data.OleDb;

namespace EntitySpaces.MetadataEngine.SQLite
{
	public class SQLiteColumn : Column
	{
		internal bool _isAutoKey = false;
		internal int _autoInc   = 0;
		internal int _autoSeed  = 0;

        internal bool _isComputed = false;

        public SQLiteColumn()
		{

		}

		override internal Column Clone()
		{
			Column c = base.Clone();

			return c;
		}

        public override bool IsComputed
        {
            get { return this._isComputed; }
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
				SQLiteColumns cols = Columns as SQLiteColumns;
				return this.GetString(cols.f_TypeName);
			}
		}

		override public string DataTypeNameComplete
		{
			get
			{
				SQLiteColumns cols = Columns as SQLiteColumns;
                string result = this.GetString(cols.f_TypeNameComplete).Replace("\'", string.Empty);
                // Depuración: escribe en un archivo o en la consola
                System.Diagnostics.Debug.WriteLine($"Column: {this.Name}, DataTypeNameComplete: {result}");
                return result;
            }
		}

        // ============================================================
        // START: Metadata improvements (Accuracy, Scale, Length)
        // ============================================================

        public override Int32 CharacterMaxLength
        {
            get
            {
                string type = this.DataTypeNameComplete.ToUpper();
                if (!type.StartsWith("VARCHAR") && !type.StartsWith("CHAR")) return 0;
                int start = type.IndexOf('(');
                if (start == -1) return 0;
                int end = type.IndexOf(')');
                if (end == -1 || end <= start) return 0;
                string lenStr = type.Substring(start + 1, end - start - 1).Trim();
                if (int.TryParse(lenStr, out int l)) return l;
                return 0;
            }
        }

        public override Int32 NumericPrecision
        {
            get
            {
                string type = this.DataTypeNameComplete.ToUpper();
                int start = type.IndexOf('(');
                if (start == -1) return 0;
                int end = type.IndexOf(',');
                if (end == -1) end = type.IndexOf(')');
                if (end == -1 || end <= start) return 0;
                string precisionStr = type.Substring(start + 1, end - start - 1).Trim();
                if (int.TryParse(precisionStr, out int p)) return p;
                return 0;
            }
        }

        public override Int32 NumericScale
        {
            get
            {
                string type = this.DataTypeNameComplete.ToUpper();
                int start = type.IndexOf(',');
                if (start == -1) return 0;
                int end = type.IndexOf(')');
                if (end == -1 || end <= start) return 0;
                string scaleStr = type.Substring(start + 1, end - start - 1).Trim();
                if (int.TryParse(scaleStr, out int s)) return s;
                return 0;
            }
        }


        // ============================================================
        // END: Metadata improvements
        // ============================================================
    }
}

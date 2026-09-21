using System;
using System.Collections;
using System.Data;

namespace EntitySpaces.MetadataEngine.Sql
{
    /// <summary>
    /// Summary description for DatabaseSpecific.
    /// </summary>
    public class DatabaseSpecific
    {
        public const string EXTENDED_PROPERTIES = "ExtendedProperties";
        private const string QUERY = @"SELECT [name], [value] FROM ::fn_listextendedproperty (NULL, 'user', {0}, {1}, {2}, {3}, {4})";

        public DatabaseSpecific() { }

        public KeyValueCollection ExtendedProperties(IColumn column)
        {
            if (column.Table != null)
            {
                return ExtendedProperties(column.Table.Database, column.Table.Schema, "table", column.Table.Name, column.Name);
            }
            else
            {
                return ExtendedProperties(column.View.Database, column.View.Schema, "view", column.View.Name, column.Name);
            }
        }

        public KeyValueCollection ExtendedProperties(ITable table)
        {
            return ExtendedProperties(table.Database, table.Schema, "table", table.Name, null);
        }

        public KeyValueCollection ExtendedProperties(IProcedure proc)
        {
            return ExtendedProperties(proc.Database, proc.Schema, "procedure", proc.Name, null);
        }

        public KeyValueCollection ExtendedProperties(IView view)
        {
            return ExtendedProperties(view.Database, view.Schema, "view", view.Name, null);
        }

        private KeyValueCollection ExtendedProperties(IDatabase db, string schema, string entitytype, string entity, string column)
        {
            KeyValueCollection hash = new KeyValueCollection();

            DataTable dt = db.ExecuteSql(
                String.Format(QUERY,
                "'" + schema + "'",
                "'" + entitytype + "'",
                "'" + entity + "'",
                ((column == null) ? "null" : "'column'"),
                ((column == null) ? "null" : "'" + column + "'")
                )
            );

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string name = row["name"] != DBNull.Value ? row["name"].ToString() : string.Empty;
                    string value = row["value"] != DBNull.Value ? row["value"].ToString() : string.Empty;

                    hash.AddKeyValue(name, value);
                }
            }

            return hash;
        }
    }
}
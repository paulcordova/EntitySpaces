using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace EntitySpaces.MetadataEngine.SQLite
{
    public class SQLiteForeignKeys : ForeignKeys
    {
        public SQLiteForeignKeys()
        {
        }

        override internal void LoadAll()
        {
            IDbConnection cn = null;

            try
            {
                cn = ConnectionHelper.CreateConnection(this.dbRoot, this.Table.Database.Name);

                string catalog   = this.Table.Database.Name;
                string thisTable = this.Table.Name;

                // Collect FK rows and their column mappings across all tables.
                // We must scan every table because we need FKs in both directions:
                //   - FKs where thisTable is the child  (FK side): from PRAGMA foreign_key_list(thisTable)
                //   - FKs where thisTable is the parent (PK side): scan all tables looking for references to thisTable
                DataTable metaData = BuildEmptyFkTable();
                var fkColumns = new Dictionary<string, (List<string> pkCols, List<string> fkCols)>(
                    StringComparer.OrdinalIgnoreCase);

                // Get all user tables in the database
                DataTable allTables = new DataTable();
                using (DbDataAdapter ta = SQLiteDatabases.CreateAdapter(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'", cn))
                {
                    ta.Fill(allTables);
                }

                foreach (DataRow tableRow in allTables.Rows)
                {
                    string tbl = tableRow["name"] as string;

                    // PRAGMA foreign_key_list(T) returns one row per FK column:
                    //   id | seq | table | from | to | on_update | on_delete | match
                    //   id    — FK constraint index within tbl (groups multi-column FKs)
                    //   seq   — 0-based position within the FK
                    //   table — referenced (PK) table
                    //   from  — column in tbl (FK side)
                    //   to    — column in referenced table (PK side); empty = use PK of that table
                    DataTable pragma = new DataTable();
                    using (DbDataAdapter pa = SQLiteDatabases.CreateAdapter(
                        $"PRAGMA foreign_key_list(\"{tbl}\")", cn))
                    {
                        pa.Fill(pragma);
                    }

                    if (pragma.Rows.Count == 0) continue;

                    // Group pragma rows by FK id — each id is one FK constraint
                    var groups = new SortedDictionary<int, List<DataRow>>();
                    foreach (DataRow pr in pragma.Rows)
                    {
                        int id = Convert.ToInt32(pr["id"]);
                        if (!groups.ContainsKey(id)) groups[id] = new List<DataRow>();
                        groups[id].Add(pr);
                    }

                    foreach (var kv in groups)
                    {
                        var rows = kv.Value;
                        rows.Sort((a, b) => Convert.ToInt32(a["seq"]).CompareTo(Convert.ToInt32(b["seq"])));

                        string pkTable   = rows[0]["table"]     as string;
                        string updateRule = rows[0]["on_update"] as string ?? "NO ACTION";
                        string deleteRule = rows[0]["on_delete"] as string ?? "NO ACTION";

                        // Only include FKs that involve thisTable on either side
                        bool isFkSide = string.Equals(tbl,     thisTable, StringComparison.OrdinalIgnoreCase);
                        bool isPkSide = string.Equals(pkTable, thisTable, StringComparison.OrdinalIgnoreCase);
                        if (!isFkSide && !isPkSide) continue;

                        // Stable FK name: FK_childTable_parentTable_id
                        string fkName = $"FK_{tbl}_{pkTable}_{kv.Key}";
                        if (fkColumns.ContainsKey(fkName)) continue; // avoid duplicate from both-side scan

                        DataRow fkRow = metaData.NewRow();
                        fkRow["FK_NAME"]         = fkName;
                        fkRow["PK_TABLE_SCHEMA"] = string.Empty;
                        fkRow["PK_TABLE_NAME"]   = pkTable;
                        fkRow["FK_TABLE_SCHEMA"] = string.Empty;
                        fkRow["FK_TABLE_NAME"]   = tbl;
                        fkRow["UPDATE_RULE"]     = updateRule;
                        fkRow["DELETE_RULE"]     = deleteRule;
                        metaData.Rows.Add(fkRow);

                        var pkCols = new List<string>();
                        var fkCols = new List<string>();
                        foreach (DataRow r in rows)
                        {
                            fkCols.Add(r["from"] as string ?? string.Empty);
                            string toCol = r["to"] as string;
                            if (string.IsNullOrEmpty(toCol))
                                toCol = ResolvePkColumn(cn, pkTable); // implicit FK to PK
                            pkCols.Add(toCol);
                        }
                        fkColumns[fkName] = (pkCols, fkCols);
                    }
                }

                // Populate the base collection
                PopulateArrayNoHookup(metaData);

                // Wire up column mappings on each loaded ForeignKey object
                foreach (ForeignKey key in this)
                {
                    if (!fkColumns.TryGetValue(key.Name, out var colMaps)) continue;

                    string pkTableName = key._row["PK_TABLE_NAME"] as string;
                    string fkTableName = key._row["FK_TABLE_NAME"] as string;

                    for (int i = 0; i < colMaps.pkCols.Count; i++)
                    {
                        key.AddForeignColumn(catalog, string.Empty, pkTableName, colMaps.pkCols[i], true);
                        key.AddForeignColumn(catalog, string.Empty, fkTableName, colMaps.fkCols[i], false);
                    }
                }

                cn.Close();
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                if (cn != null && cn.State == ConnectionState.Open)
                    cn.Close();
            }
        }

        // Build the minimal DataTable schema that PopulateArrayNoHookup requires.
        private static DataTable BuildEmptyFkTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("FK_NAME",         typeof(string));
            dt.Columns.Add("PK_TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("PK_TABLE_NAME",   typeof(string));
            dt.Columns.Add("FK_TABLE_SCHEMA", typeof(string));
            dt.Columns.Add("FK_TABLE_NAME",   typeof(string));
            dt.Columns.Add("UPDATE_RULE",     typeof(string));
            dt.Columns.Add("DELETE_RULE",     typeof(string));
            return dt;
        }

        // When PRAGMA foreign_key_list returns an empty "to" column, the FK
        // references the PK of the parent table implicitly. This method resolves
        // the actual PK column name via PRAGMA table_info.
        private static string ResolvePkColumn(IDbConnection cn, string tableName)
        {
            DataTable pragma = new DataTable();
            using (DbDataAdapter pa = SQLiteDatabases.CreateAdapter(
                $"PRAGMA table_info(\"{tableName}\")", cn))
            {
                pa.Fill(pragma);
            }

            foreach (DataRow r in pragma.Rows)
            {
                int pk = r["pk"] != DBNull.Value ? Convert.ToInt32(r["pk"]) : 0;
                if (pk == 1) // 1-based; first column of the PK
                    return r["name"] as string ?? string.Empty;
            }
            return string.Empty;
        }
    }
}

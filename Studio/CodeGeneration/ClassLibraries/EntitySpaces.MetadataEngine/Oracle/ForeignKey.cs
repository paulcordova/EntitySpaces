using System;
using System.Data;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleForeignKey : ForeignKey
    {
        public OracleForeignKey()
        {
        }

        internal override void AddForeignColumn(string catalog, string schema,
            string physicalTableName, string physicalColumnName, bool primary)
        {
            string logPath = @"C:\oracle\fk_debug.txt";
            try
            {
                using (StreamWriter sw = new StreamWriter(logPath, true))
                    sw.WriteLine("  AddForeignColumn: catalog='" + catalog + "' schema='" + schema +
                        "' table='" + physicalTableName + "' col='" + physicalColumnName +
                        "' primary=" + primary);

                base.AddForeignColumn(catalog, schema, physicalTableName, physicalColumnName, primary);

                using (StreamWriter sw = new StreamWriter(logPath, true))
                    sw.WriteLine("  AddForeignColumn OK: PrimaryColumns=" +
                        (_primaryColumns == null ? "NULL" : _primaryColumns.Count.ToString()) +
                        " ForeignColumns=" +
                        (_foreignColumns == null ? "NULL" : _foreignColumns.Count.ToString()));
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(logPath, true))
                    sw.WriteLine("  AddForeignColumn EXCEPTION: " + ex.Message + "\n  " + ex.StackTrace);
            }
        }
    }
}

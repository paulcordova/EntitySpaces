using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace EntitySpaces.MetadataEngine.SQLite
{
    public class SQLiteDatabases : Databases
    {
        // System.Data.SQLite.Core uses namespace "System.Data.SQLite"
        // and assembly name "System.Data.SQLite".
        static internal string nameSpace = "System.Data.SQLite.";
        static internal Assembly asm = null;
        static internal Module   mod = null;

        static internal ConstructorInfo IDbConnectionCtor  = null;
        static internal ConstructorInfo IDbDataAdapterCtor  = null;
        static internal ConstructorInfo IDbDataAdapterCtor2 = null;

        internal string Version = "";

        public SQLiteDatabases()
        {
            SQLiteDatabases.LoadAssembly();
        }

        static SQLiteDatabases()
        {
            LoadAssembly();
        }

        static public void LoadAssembly()
        {
            try
            {
                if (asm == null)
                {
                    try
                    {
                        // System.Data.SQLite.Core NuGet package assembly name
                        asm = Assembly.Load("System.Data.SQLite");
                        if (asm == null)
                            throw new Exception("Assembly 'System.Data.SQLite' returned null from LoadWithPartialName.");

                        Module[] mods = asm.GetModules(false);
                        mod = mods[0];
                    }
                    catch
                    {
                        throw new Exception(
                            "Make sure System.Data.SQLite.dll is in the application directory. " +
                            "Install NuGet package System.Data.SQLite.Core in this project.");
                    }
                }
            }
            catch { }
        }

        override internal void LoadAll()
        {

            // List all user tables — used to populate the Databases tree node.
            // Exclude internal sqlite_* tables (sqlite_sequence, sqlite_master, etc.)
            string query = "SELECT name AS TABLE_NAME FROM sqlite_master " +
                           "WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name;";

            DbDataAdapter adapter = SQLiteDatabases.CreateAdapter(query, this.dbRoot.ConnectionString);
            DataTable metaData = new DataTable();
            adapter.Fill(metaData);
            PopulateArray(metaData);
        }

        // Previously looked for "SQLite.SQLitesqlConnection" — a typo inherited from
        // a Sybase provider — which silently returned null and broke all metadata queries.
        static internal IDbConnection CreateConnection(string connStr)
        {
            if (IDbConnectionCtor == null)
            {
                Type type = mod.GetType(nameSpace + "SQLiteConnection");
                IDbConnectionCtor = type.GetConstructor(new Type[] { typeof(string) });
            }

            object obj = IDbConnectionCtor.Invoke(
                BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
                null, new object[] { connStr }, null);

            return obj as IDbConnection;
        }

        static internal DbDataAdapter CreateAdapter(string query, string connStr)
        {
            if (IDbDataAdapterCtor == null)
            {
                Type type = mod.GetType(nameSpace + "SQLiteDataAdapter");
                IDbDataAdapterCtor = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
            }

            object obj = IDbDataAdapterCtor.Invoke(
                BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
                null, new object[] { query, connStr }, null);

            return obj as DbDataAdapter;
        }

        static internal DbDataAdapter CreateAdapter(string query, IDbConnection conn)
        {
            if (IDbDataAdapterCtor2 == null)
            {
                Type type = mod.GetType(nameSpace + "SQLiteDataAdapter");
                // Find constructor (string, SQLiteConnection)
                foreach (ConstructorInfo ci in type.GetConstructors())
                {
                    ParameterInfo[] p = ci.GetParameters();
                    if (p.Length == 2 && p[0].ParameterType == typeof(string))
                    {
                        IDbDataAdapterCtor2 = ci;
                        break;
                    }
                }
            }

            object obj = IDbDataAdapterCtor2.Invoke(
                BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
                null, new object[] { query, conn }, null);

            return obj as DbDataAdapter;
        }
    }
}

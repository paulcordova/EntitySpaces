using EntitySpaces.MetadataEngine.MySql;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace EntitySpaces.MetadataEngine.Firebird
{
    public class FirebirdDatabases : Databases
    {
        static internal string nameSpace = "FirebirdSql.Data.FirebirdClient";
        static internal Assembly asm = null;
        static internal Module mod = null;

        static internal ConstructorInfo IDbConnectionCtor = null;
        static internal ConstructorInfo IDbDataAdapterCtor = null;
        static internal ConstructorInfo IDbDataAdapterCtor2 = null;

        internal string Version = "";

        public FirebirdDatabases()
        {

        }

        static FirebirdDatabases()
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
                        asm = Assembly.Load("FirebirdSql.Data.FirebirdClient");
                        Module[] mods = asm.GetModules(false);
                        mod = mods[0];
                    }
                    catch
                    {
                        throw new Exception("Make sure the Firebird.dll is registered in the Gac or is located in the MyGeneration folder.");
                    }
                }
            }
            catch { }
        }

        override internal void LoadAll()
        {

            try
            {
                string name = "";

                // We add our one and only Database
                IDbConnection conn = FirebirdDatabases.CreateConnection(this.dbRoot.ConnectionString);
                conn.Open();
                name = conn.Database;
                conn.Close();
                conn.Dispose();

                FirebirdDatabase database = (FirebirdDatabase)this.dbRoot.ClassFactory.CreateDatabase();
                database._name = name;
                database.dbRoot = this.dbRoot;
                database.Databases = this;
                this._array.Add(database);

                try
                {
                    DataTable metaData = new DataTable();
                    string sql = "SELECT RDB$GET_CONTEXT('SYSTEM', 'ENGINE_VERSION') AS VERSION FROM RDB$DATABASE;";
                    DbDataAdapter adapter = FirebirdDatabases.CreateAdapter(sql, this.dbRoot.ConnectionString);


                    adapter.Fill(metaData);

                    this.Version = metaData.Rows[0][0] as string;
                }
                catch { }
            }
            catch { }

        }


        static internal IDbConnection CreateConnection(string connStr)
        {
            if (IDbConnectionCtor == null)
            {
                Type type = mod.GetType(nameSpace + ".FbConnection");

                IDbConnectionCtor = type.GetConstructor(new Type[] { typeof(string) });
            }

            object obj = IDbConnectionCtor.Invoke(BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding,
                null, new object[] { connStr }, null);

            return obj as IDbConnection;
        }

        static internal DbDataAdapter CreateAdapter(string query, string connStr)
        {
            if (IDbDataAdapterCtor == null)
            {
                Type type = mod.GetType(nameSpace + ".FbDataAdapter");

                IDbDataAdapterCtor = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
            }

            object obj = IDbDataAdapterCtor.Invoke
                (BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding, null,
                new object[] { query, connStr }, null);

            return obj as DbDataAdapter;
        }

        static internal DbDataAdapter CreateAdapter(string query, IDbConnection conn)
        {
            if (IDbDataAdapterCtor2 == null)
            {
                Type type = mod.GetType(nameSpace + ".FbDataAdapter");

                ConstructorInfo[] ctrs = type.GetConstructors();

                IDbDataAdapterCtor2 = ctrs[3];
            }

            object obj = IDbDataAdapterCtor2.Invoke
                (BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding, null,
                new object[] { query, conn }, null);

            return obj as DbDataAdapter;
        }
        

    }
}

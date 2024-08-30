using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleDatabases : Databases
    {
        static internal string nameSpace = "Oracle.";
        static internal Assembly asm = null;
        static internal Module mod = null;

        static internal ConstructorInfo IDbConnectionCtor = null;
        static internal ConstructorInfo IDbDataAdapterCtor = null;
        static internal ConstructorInfo IDbDataAdapterCtor2 = null;

        internal string Version = "";

        public OracleDatabases()
        {
        }

        static OracleDatabases()
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
                        asm = Assembly.Load("Oracle.ManagedDataAccess");

                        Module[] mods = asm.GetModules(false);
                        mod = mods[0];
                    }
                    catch
                    {
                        throw new Exception("Make sure the Oracle.ManagedDataAccess.dll is registered in the Gac or is located in the MyGeneration folder.");
                    }
                }
            }
            catch { }
        }

        
        override internal void LoadAll()
        {

            string query = @"SELECT
                               username  AS CATALOG_NAME,
                               sys_context('userenv', 'CURRENT_SCHEMA') AS SCHEMA_OWNER,
                               sys_context('userenv', 'CURRENT_SCHEMA') AS SCHEMA_NAME
                            FROM
                                v$database, 
                                all_users
                            WHERE username =  sys_context('userenv', 'CURRENT_SCHEMA')  
                            ORDER BY
                                name";

            using (DbDataAdapter adapter = Oracle.OracleDatabases.CreateAdapter(query, this.dbRoot.ConnectionString))
            {
                DataTable metaDataOracle = new DataTable();

                adapter.Fill(metaDataOracle);

                PopulateArray(metaDataOracle);

            }
                        
        }

        static internal IDbConnection CreateConnection(string connStr)
        {
            if (IDbConnectionCtor == null)
            {
                Type type = mod.GetType(nameSpace + "ManagedDataAccess.Client.OracleConnection");

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
                Type type = mod.GetType(nameSpace + "ManagedDataAccess.Client.OracleDataAdapter");

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
                Type type = mod.GetType(nameSpace + "ManagedDataAccess.Client.OracleDataAdapter");

                ConstructorInfo[] ctrs = type.GetConstructors();

                IDbDataAdapterCtor2 = ctrs[2];
            }

            object obj = IDbDataAdapterCtor2.Invoke
                (BindingFlags.CreateInstance | BindingFlags.OptionalParamBinding, null,
                new object[] { query, conn }, null);

            return obj as DbDataAdapter;
        }




    } // end class
} // end namespace
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;

namespace EntitySpaces.MetadataEngine.Sql
{
    public class SqlResultColumns : ResultColumns
    {
        public SqlResultColumns()
        {

        }

        override internal void LoadAll()
        {
            try
            {
                string schema = "";

                if (-1 == this.Procedure.Schema.IndexOf("."))
                {
                    schema = this.Procedure.Schema + ".";
                }

                string select = "EXEC [" + this.Procedure.Database.Name + "]." + schema + "[" +
                    this.Procedure.Name + "] ";

                int paramCount = this.Procedure.Parameters.Count;

                if (paramCount > 0)
                {
                    IParameters parameters = this.Procedure.Parameters;
                    IParameter param = null;

                    int c = parameters.Count;

                    for (int i = 0; i < c; i++)
                    {
                        param = parameters[i];

                        if (param.Direction == ParamDirection.ReturnValue)
                        {
                            paramCount--;
                        }
                    }
                }

                for (int i = 0; i < paramCount; i++)
                {
                    if (i > 0)
                    {
                        select += ",";
                    }

                    select += "null";
                }

                DataTable metaData = new DataTable();

                try
                {
                    string[] pairs = dbRoot.ConnectionString.Split(';');
                    Hashtable conn = new Hashtable();
                    int idx;
                    string name, val;
                    foreach (string pairstr in pairs)
                    {
                        idx = pairstr.IndexOf('=');
                        if (idx > 0)
                        {
                            name = pairstr.Substring(0, idx);
                            val = pairstr.Substring(idx + 1);
                            conn[name.Trim()] = val.Trim();
                        }
                    }

                    string cn = "";
                    foreach (string key in conn.Keys)
                    {
                        string tmp = conn[key] as string;
                        switch (key.ToLower())
                        {
                            case "provider":
                            case "extended properties":
                            case "persist security info":
                                break;
                            case "server":
                            case "data source":
                            case "address":
                            case "addr":
                                cn += "Data Source=" + tmp + ";";
                                break;
                            case "user id":
                            case "uid":
                                cn += "User ID=" + tmp + ";";
                                break;
                            case "password":
                            case "pwd":
                                cn += "Password=" + tmp + ";";
                                break;
                            case "initial catalog":
                            case "database":
                                cn += "Initial Catalog=" + tmp + ";";
                                break;
                            case "marsconn":
                            case "multipleactiveresultsets":
                                cn += "MultipleActiveResultSets=" + (tmp.ToLower() == "yes" || tmp.ToLower() == "true" ? "true" : "false") + ";";
                                break;
                            case "integrated security":
                            case "trusted_connection":
                                cn += "Integrated Security=" + tmp + ";";
                                break;
                            default:
                                cn += key + "=" + tmp + ";";
                                break;
                        }
                    }

                    // Asegurar valores por defecto si falta seguridad integrada o usuario
                    if (!cn.Contains("Integrated Security") && !cn.Contains("User ID"))
                    {
                        cn += "Integrated Security=true;TrustServerCertificate=true;";
                    }
                    else if (!cn.Contains("TrustServerCertificate"))
                    {
                        cn += "TrustServerCertificate=true;";
                    }

                    using (SqlConnection sqlconn = new SqlConnection(cn))
                    {
                        sqlconn.Open();
                        using (SqlCommand sqlcmd = sqlconn.CreateCommand())
                        {
                            sqlcmd.CommandText = select;
                            sqlcmd.CommandType = CommandType.Text;
                            using (SqlDataReader reader = sqlcmd.ExecuteReader(CommandBehavior.SchemaOnly))
                            {
                                metaData = reader.GetSchemaTable();
                            }
                        }
                    }

                    if (metaData != null)
                    {
                        SqlResultColumn resultColumn;
                        foreach (DataRow row in metaData.Rows)
                        {
                            resultColumn = this.dbRoot.ClassFactory.CreateResultColumn() as Sql.SqlResultColumn;
                            resultColumn.dbRoot = this.dbRoot;
                            resultColumn.ResultColumns = this;
                            resultColumn._row = row;
                            this._array.Add(resultColumn);
                        }
                    }
                }
                catch (Exception cx)
                {
                    // Opcional: puedes dejar un registro de excepción aquí si falla la conexión para depurar
                }
            }
            catch { }
        }
    }
}
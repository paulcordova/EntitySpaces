using System;
using Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    public class OracleView : View
    {
        public OracleView()
        {
        }

        public override string ViewText
        {
            get
            {
                string tmp = base.ViewText;
                if (tmp.Length == 0)
                {
                    tmp = LoadViewSource();
                }
                return tmp;
            }
        }

        private string LoadViewSource()
        {
            string text = string.Empty;

            // Query to retrieve the view definition in Oracle
            string select = "SELECT TEXT FROM ALL_VIEWS WHERE VIEW_NAME = :viewName AND OWNER = :owner";

            try
            {
                // Use the global:: prefix to explicitly reference the Oracle assembly
                using (global::Oracle.ManagedDataAccess.Client.OracleConnection cn = new global::Oracle.ManagedDataAccess.Client.OracleConnection(this.dbRoot.ConnectionString))
                {
                    cn.Open();
                    using (global::Oracle.ManagedDataAccess.Client.OracleCommand cmd = new global::Oracle.ManagedDataAccess.Client.OracleCommand(select, cn))
                    {
                        cmd.BindByName = true;

                        // Use global:: to ensure we reference the assembly library correctly
                        cmd.Parameters.Add(new global::Oracle.ManagedDataAccess.Client.OracleParameter("viewName", this.Name));
                        cmd.Parameters.Add(new global::Oracle.ManagedDataAccess.Client.OracleParameter("owner", this.Database.SchemaOwner));

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            text = result.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                Console.WriteLine("Error loading view source: " + ex.Message);
            }

            return text;
        }

    } // end class
} //end namespace
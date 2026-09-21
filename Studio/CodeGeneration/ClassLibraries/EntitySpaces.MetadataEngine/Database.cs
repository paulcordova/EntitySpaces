using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;

namespace EntitySpaces.MetadataEngine
{
    public class Database : Single, IDatabase, INameValueItem
    {
        public Database()
        {

        }

        virtual public DataTable ExecuteSql(string sql)
        {
            DataTable dataTable = new DataTable();
            OleDbConnection cn = null;
            OleDbDataReader reader = null;

            try
            {
                cn = new OleDbConnection(dbRoot.ConnectionString);
                cn.Open();
                try
                {
                    cn.ChangeDatabase(this.Name);
                }
                catch { } // some databases don't have the concept of catalogs. Catch this and throw it out

                OleDbCommand command = new OleDbCommand(sql, cn);
                command.CommandType = CommandType.Text;

                reader = command.ExecuteReader();

                // Carga directamente el esquema y los datos en el DataTable de forma nativa
                dataTable.Load(reader);

                cn.Close();
            }
            catch (Exception ex)
            {
                if ((reader != null) && (!reader.IsClosed))
                {
                    reader.Close();
                    reader = null;
                }
                if ((cn != null) && (cn.State == ConnectionState.Open))
                {
                    cn.Close();
                    cn = null;
                }
                throw ex;
            }

            return dataTable.Rows.Count > 0 ? dataTable : null;
        }

        protected DataTable ExecuteIntoRecordset(string sql, IDbConnection cn)
        {
            DataTable dataTable = new DataTable();
            IDataReader reader = null;

            try
            {
                IDbCommand command = cn.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                reader = command.ExecuteReader();

                // Carga nativa de IDataReader hacia DataTable en .NET 8
                dataTable.Load(reader);

                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                if ((reader != null) && (!reader.IsClosed))
                {
                    reader.Close();
                    reader = null;
                }
                if ((cn != null) && (cn.State == ConnectionState.Open))
                {
                    cn.Close();
                    cn = null;
                }
                throw ex;
            }

            return dataTable.Rows.Count > 0 ? dataTable : null;
        }

        // NOTA: El método GetADOType ya no es necesario porque .NET y DataTable 
        // manejan los tipos de datos nativos de System.Type directamente.

        virtual public ITables Tables
        {
            get
            {
                if (null == _tables)
                {
                    _tables = (Tables)this.dbRoot.ClassFactory.CreateTables();
                    _tables.dbRoot = this._dbRoot;
                    _tables.Database = this;
                    _tables.LoadAll();
                }

                return _tables;
            }
        }

        virtual public IViews Views
        {
            get
            {
                if (null == _views)
                {
                    _views = (Views)this.dbRoot.ClassFactory.CreateViews();
                    _views.dbRoot = this._dbRoot;
                    _views.Database = this;
                    _views.LoadAll();
                }

                return _views;
            }
        }

        [Browsable(false)]
        virtual public IProcedures Procedures
        {
            get
            {
                if (null == _procedures)
                {
                    _procedures = (Procedures)this.dbRoot.ClassFactory.CreateProcedures();
                    _procedures.dbRoot = this._dbRoot;
                    _procedures.Database = this;
                    _procedures.LoadAll();
                }

                return _procedures;
            }
        }

        [Browsable(false)]
        virtual public IDomains Domains
        {
            get
            {
                if (null == _domains)
                {
                    _domains = (Domains)this.dbRoot.ClassFactory.CreateDomains();
                    _domains.dbRoot = this._dbRoot;
                    _domains.Database = this;
                    _domains.LoadAll();
                }

                return _domains;
            }
        }

        override public string Alias
        {
            get
            {
                XmlNode node = null;
                if (this.GetXmlNode(out node, false))
                {
                    string niceName = null;

                    if (this.GetUserData(node, "Alias", out niceName))
                    {
                        if (string.Empty != niceName)
                            return niceName;
                    }
                }

                // There was no nice name
                return this.Name;
            }

            set
            {
                XmlNode node = null;
                if (this.GetXmlNode(out node, true))
                {
                    this.SetUserData(node, "Alias", value);
                }
            }
        }

        override public string Name
        {
            get
            {
                return this.GetString(Databases.f_Catalog);
            }
        }

        virtual public string Description
        {
            get
            {
                return this.GetString(Databases.f_Description);
            }
        }

        virtual public string SchemaName
        {
            get
            {
                return this.GetString(Databases.f_SchemaName);
            }
        }

        virtual public string SchemaOwner
        {
            get
            {
                return this.GetString(Databases.f_SchemaOwner);
            }
        }

        [Browsable(false)]
        virtual public string DefaultCharSetCatalog
        {
            get
            {
                return this.GetString(Databases.f_DefCharSetCat);
            }
        }

        [Browsable(false)]
        virtual public string DefaultCharSetSchema
        {
            get
            {
                return this.GetString(Databases.f_DefCharSetSchema);
            }
        }

        [Browsable(false)]
        virtual public string DefaultCharSetName
        {
            get
            {
                return this.GetString(Databases.f_DefCharSetName);
            }
        }

        [Browsable(false)]
        virtual public Root Root
        {
            get
            {
                return this.dbRoot;
            }
        }

        #region XML User Data

        [Browsable(false)]
        override public string UserDataXPath
        {
            get
            {
                return Databases.UserDataXPath + @"/Database[@Name='" + this.Name + "']";
            }
        }

        override internal bool GetXmlNode(out XmlNode node, bool forceCreate)
        {
            node = null;
            bool success = false;

            if (null == _xmlNode)
            {
                // Get the parent node
                XmlNode parentNode = null;
                if (this.Databases.GetXmlNode(out parentNode, forceCreate))
                {
                    // See if our user data already exists
                    string xPath = @"./Database[@Name='" + this.Name + "']";
                    if (!GetUserData(xPath, parentNode, out _xmlNode) && forceCreate)
                    {
                        // Create it, and try again
                        this.CreateUserMetaData(parentNode);
                        GetUserData(xPath, parentNode, out _xmlNode);
                    }
                }
            }

            if (null != _xmlNode)
            {
                node = _xmlNode;
                success = true;
            }

            return success;
        }

        override public void CreateUserMetaData(XmlNode parentNode)
        {
            XmlNode myNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Database", null);
            parentNode.AppendChild(myNode);

            XmlAttribute attr;

            attr = parentNode.OwnerDocument.CreateAttribute("Name");
            attr.Value = this.Name;
            myNode.Attributes.Append(attr);
        }

        #endregion

        #region INameValueCollection Members

        [Browsable(false)]
        public string ItemName
        {
            get
            {
                return this.Name;
            }
        }

        [Browsable(false)]
        public string ItemValue
        {
            get
            {
                return this.Name;
            }
        }

        #endregion

        internal Databases Databases = null;
        protected Tables _tables = null;
        protected Views _views = null;
        protected Procedures _procedures = null;
        protected Domains _domains = null;

        // Global properties are per Database
        internal PropertyCollection _columnProperties = null;
        internal PropertyCollection _databaseProperties = null;
        internal PropertyCollection _foreignkeyProperties = null;
        internal PropertyCollection _indexProperties = null;
        internal PropertyCollection _parameterProperties = null;
        internal PropertyCollection _procedureProperties = null;
        internal PropertyCollection _resultColumnProperties = null;
        internal PropertyCollection _tableProperties = null;
        internal PropertyCollection _viewProperties = null;
        internal PropertyCollection _domainProperties = null;
    }
}
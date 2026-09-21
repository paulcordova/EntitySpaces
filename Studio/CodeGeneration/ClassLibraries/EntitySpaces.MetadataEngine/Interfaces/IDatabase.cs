using System;
using System.Data;

namespace EntitySpaces.MetadataEngine
{
    /// <summary>
    /// IDatabase represents a database in your DBMS.
    /// </summary>
    /// <remarks>
    ///	IDatabase has 5 Collections:
    /// <list type="table">
    ///		<item><term>Tables</term><description>Contains all of the tables for the database</description></item>
    ///		<item><term>Views</term><description>Contains all of the views the database</description></item>
    ///		<item><term>Procedures</term><description>Contains all of the procedures for the database</description></item>
    ///		<item><term>Domains</term><description>Contains all of the domains for the database</description></item>
    ///		<item><term>Properties</term><description>A collection that can hold key/value pairs of your choosing</description></item>
    ///	</list>
    /// </remarks>
    public interface IDatabase
    {
        // Collections
        /// <summary>
        /// Contains all of the tables for the database
        /// </summary>
        ITables Tables { get; }

        /// <summary>
        /// Contains all of the views the database
        /// </summary>
        IViews Views { get; }

        /// <summary>
        /// Contains all of the procedures for the database
        /// </summary>
        IProcedures Procedures { get; }

        /// <summary>
        /// A link back to the dbRoot object
        /// </summary>
        Root Root { get; }

        /// <summary>
        /// Contains all of the domains for the database
        /// </summary>
        IDomains Domains { get; }

        /// <summary>
        /// The Properties for this Database. These are user defined and are typically stored in 'UserMetaData.xml' unless changed in the Default Settings dialog.
        /// Properties consist of key/value pairs. You can populate this collection during your script or via the Dockable window. 
        /// To save any data added to this collection call esMetadataEngine.SaveUserMetaData(). See <see cref="IProperty"/>
        /// </summary>
        IPropertyCollection Properties { get; }


        // User Meta Data
        string UserDataXPath { get; }

        /// <summary>
        /// You can override the physical name of the Column. If you do not provide an Alias the value of 'Column.Name' is returned.
        /// If your column in your DBMS is 'TXT_FIRST_NAME' you might want to give it an Alias of 'FirstName' so that your business object property will be a nice name.
        /// You can provide an Alias in the User Meta Data window. You can also set this during a script and then call esMetadataEngine.SaveUserMetaData().
        /// See <see cref="Name"/>
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// This is the physical column name as stored in your DBMS system. See <see cref="Alias"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Human-readable description of the database.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Unqualified schema name.
        /// </summary>
        string SchemaName { get; }

        /// <summary>
        /// User that owns the schemas.
        /// </summary>
        string SchemaOwner { get; }

        /// <summary>
        /// Catalog name of the default character set for columns and domains in the schemas. 
        /// Blank if the provider does not support catalogs or different character sets.
        /// </summary>
        string DefaultCharSetCatalog { get; }

        /// <summary>
        /// Unqualified schema name of the default character set for columns and domains in the schemas. 
        /// Blank if the provider does not support different character sets.
        /// </summary>
        string DefaultCharSetSchema { get; }

        /// <summary>
        /// Default character set name. Blank if the provider does not support different character sets.
        /// </summary>
        string DefaultCharSetName { get; }

        // Methods
        /// <summary>
        /// This method can execute any SQL statement against your DBMS system, including SELECT, INSERT, UPDATE, DELETE and more.  
        /// </summary>
        /// <param name="sql">Raw SQL statement to be executed. Returns a standard .NET DataTable containing the result set.</param>
        /// <returns>A DataTable with the query results.</returns>
        DataTable ExecuteSql(string sql);

        /// <summary>
        /// Fetch any database specific meta data through this generic interface by key. The keys will have to be defined by the specific database provider
        /// </summary>
        /// <param name="key">A key identifying the type of meta data desired.</param>
        /// <returns>A meta-data object or collection.</returns>
        object DatabaseSpecificMetaData(string key);
    }
}
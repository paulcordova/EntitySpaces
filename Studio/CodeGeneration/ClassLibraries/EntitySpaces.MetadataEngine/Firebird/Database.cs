using System;
using System.Data;
using System.Data.OleDb;

using ADODB;

namespace EntitySpaces.MetadataEngine.Firebird
{
    public class FirebirdDatabase : Database
    {
        public FirebirdDatabase()
        {

        }

        override public string Alias
        {
            get
            {
                return _name;
            }
        }

        override public string Name
        {
            get
            {
                return _name;
            }
        }

        override public string Description
        {
            get
            {
                return _desc;
            }
        }

        internal string _name = "";
        internal string _desc = "";

        internal bool _FKsInLoad = false;
    }
}

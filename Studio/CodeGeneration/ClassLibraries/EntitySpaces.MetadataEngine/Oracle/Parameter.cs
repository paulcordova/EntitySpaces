using System;
using System.Data;
using OracleClient = Oracle.ManagedDataAccess.Client;

namespace EntitySpaces.MetadataEngine.Oracle
{
    // Extend the base Parameter class, encapsulating OracleParameter from OracleClient
    public class OracleParameter : Parameter
    {
        // Encapsulated instance of OracleParameter from Oracle.ManagedDataAccess.Client
        private OracleClient.OracleParameter _oracleParameter;
        private string _name;

        // Default constructor initializing the encapsulated OracleParameter
        public OracleParameter()
        {
            _oracleParameter = new OracleClient.OracleParameter();
        }

        // Constructor that sets the name and initializes the encapsulated OracleParameter
        public OracleParameter(string name)
        {
            _oracleParameter = new OracleClient.OracleParameter();
            this._name = name;
        }

        // Property to access or modify the Name, with custom logic
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    // Replace with appropriate logic to generate a default name
                    _name = "[" + this.Parameters.Procedure.Name + ":" + this.Ordinal.ToString() + "]";
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        // Override Direction to return a default value, or customize as needed
        public override ParamDirection Direction
        {
            get
            {
                // Use the encapsulated OracleParameter's Direction if needed
                return ParamDirection.Input; // Or customize based on _oracleParameter.Direction
            }
        }

        // Override DataTypeNameComplete to provide the desired data type name
        public override string DataTypeNameComplete
        {
            get
            {
                // Assuming OracleParameters contains a field f_TypeName to get the type name
                OracleParameters parameters = this.Parameters as OracleParameters;
                return this.GetString(parameters.f_TypeName);
            }
        }

        // Property to access the encapsulated OracleParameter object
        public OracleClient.OracleParameter InnerParameter
        {
            get { return _oracleParameter; }
        }

        // Example of how to expose additional properties or methods of OracleParameter
        public object Value
        {
            get { return _oracleParameter.Value; }
            set { _oracleParameter.Value = value; }
        }

        public override string ToString()
        {
            return _oracleParameter.ToString();
        }

        // You can add additional methods or properties that interact with _oracleParameter
    }


} // end namespace

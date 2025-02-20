/*  New BSD License
-------------------------------------------------------------------------------
Copyright (c) 2006-2012, EntitySpaces, LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the EntitySpaces, LLC nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL EntitySpaces, LLC BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
-------------------------------------------------------------------------------
*/

using System;
using System.Collections.Generic;

using EntitySpaces.Interfaces;

using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Common;


namespace EntitySpaces.FirebirdSqlProvider
{
    class Cache
    {
        static public Dictionary<string,  FbParameter> GetParameters(esDataRequest request)
        {
            return GetParameters(request.DataID, request.ProviderMetadata, request.Columns);
        }

        static public Dictionary<string, FbParameter> GetParameters(Guid dataID,
            esProviderSpecificMetadata providerMetadata, esColumnMetadataCollection columns)
        {
            lock (parameterCache)
            {
                if (!parameterCache.ContainsKey(dataID))
                {
                    // The Parameters for this Table haven't been cached yet, this is a one time operation
                    Dictionary<string, FbParameter> types = new Dictionary<string, FbParameter>();

                    FbParameter param1;
                    foreach (esColumnMetadata col in columns)
                    {
                        esTypeMap typeMap = providerMetadata.GetTypeMap(col.PropertyName);
                        if (typeMap != null)
                        {
                            string nativeType = typeMap.NativeType;
                            FbDbType  dbType = Cache.NativeTypeToDbType(nativeType);

                            param1 = new FbParameter(Delimiters.Param + col.PropertyName, dbType, 0, col.Name);
                            param1.SourceColumn = col.Name;

                            switch (dbType)
                            {
                                case FbDbType.Decimal:
                                case FbDbType.Double:
                                case FbDbType.Float:
                                case FbDbType.Integer:
                                case FbDbType.BigInt:
                                case FbDbType.SmallInt:
                                case FbDbType.Int128:
                                case FbDbType.Numeric:

                                    param1.Size = (int)col.CharacterMaxLength;
                                    param1.Precision = (byte)col.NumericPrecision;
                                    param1.Scale = (byte)col.NumericScale;
                                    break;

                                case FbDbType.VarChar:
                                case FbDbType.Char:
                                case FbDbType.Text:
                                    param1.Size = (int)col.CharacterMaxLength;
                                    break;

                            }
                            types[col.Name] = param1;
                        }
                    }

                    parameterCache[dataID] = types;
                }
            }

            return parameterCache[dataID];
        }

        static private FbDbType NativeTypeToDbType(string nativeType)
        {
            switch(nativeType.ToLower())
            {
                case "array": return FbDbType.Array;
                case "bigint": return FbDbType.BigInt;
                case "binary": return FbDbType.Binary;
                case "boolean": return FbDbType.Boolean;
                case "date": return FbDbType.Date;
                case "dec16": return FbDbType.Dec16;
                case "dec34": return FbDbType.Dec34;
                case "decimal": return FbDbType.Decimal;
                case "double": return FbDbType.Double;
                case "float": return FbDbType.Float;
                case "guid": return FbDbType.Guid;
                case "int128": return FbDbType.Int128;
                case "integer": return FbDbType.Integer;
                case "numeric": return FbDbType.Numeric;
                case "smallint": return FbDbType.SmallInt;
                case "text": return FbDbType.Text;
                case "time": return FbDbType.Time;
                case "timestamp": return FbDbType.TimeStamp;
                case "timestamptz": return FbDbType.TimeStampTZ;
                case "timestamptzex": return FbDbType.TimeStampTZEx;
                case "timetz": return FbDbType.TimeTZ;
                case "timetzex": return FbDbType.TimeTZEx;

                default:
                    return FbDbType.VarChar;
            }
        }

        static public FbParameter CloneParameter(FbParameter p)
        {
            ICloneable param = p as ICloneable;
            return param.Clone() as FbParameter;
        }
        
        static private Dictionary<Guid, Dictionary<string, FbParameter>> parameterCache
            = new Dictionary<Guid, Dictionary<string, FbParameter>>();
    }
}

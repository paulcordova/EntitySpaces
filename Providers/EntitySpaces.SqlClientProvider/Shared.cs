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
using System.Data;

#if NET48
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif


using EntitySpaces.DynamicQuery;
using EntitySpaces.Interfaces;

namespace EntitySpaces.SqlClientProvider
{
    class Shared
    {
        static public SqlCommand BuildDynamicInsertCommand(esDataRequest request, esEntitySavePacket packet)
        {
            string into = String.Empty;
            string values = String.Empty;
            string comma = String.Empty;

            // Unified OUTPUT...INTO variables — replaces old seq, autoInc, computed, and fallback SELECT
            string outputDeclare = string.Empty;
            string outputCols = string.Empty;
            string outputInto = string.Empty;
            string outputSelect = string.Empty;
            string outputComma = string.Empty;
            bool hasOutputCols = false;

            List<string> modifiedColumns = packet.ModifiedColumns;

            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            SqlParameter p = null;
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            // XACT_ABORT ensures CHECK/FK constraint errors raise SqlException in all SQL Server versions
            string sql = "SET NOCOUNT OFF; SET XACT_ABORT ON;";

            foreach (esColumnMetadata col in request.Columns)
            {
                string colName = col.Name;

                if (request.SelectedColumns != null && !request.SelectedColumns.ContainsKey(colName)) continue;

                bool isModified = modifiedColumns == null ? false : modifiedColumns.Contains(col.Name);

                if (isModified && !col.IsComputed && !col.IsConcurrency && !col.IsAutoIncrement)
                {
                    // Regular modified column — normal INSERT parameter
                    p = types[colName];
                    p = cmd.Parameters.Add(CloneParameter(p));

                    object value = packet.CurrentValues[colName];
                    p.Value = value != null ? value : DBNull.Value;

                    CreateInsertSQLSnippet(colName, p, ref into, ref values, ref comma);
                }
                else
                {
                    // Server-generated or special column — needs OUTPUT...INTO retrieval
                    bool needOutputParam = false;
                    SqlParameter clone = null;

                    if (col.HasDefault)
                    {
                        p = types[colName];

                        if (col.esType == esSystemType.Guid && col.Default.ToLower().Contains("newid"))
                        {
                            // newid() default: assign value client-side and include in INSERT,
                            // then echo it back via OUTPUT...INTO so the parameter is populated
                            sql += " SET " + p.ParameterName + " = NEWID(); ";
                            CreateInsertSQLSnippet(colName, p, ref into, ref values, ref comma);
                            needOutputParam = true;
                        }
                        else if (col.Default.ToLower().Contains("newsequentialid"))
                        {
                            // newsequentialid() is server-generated; retrieve via OUTPUT...INTO
                            needOutputParam = true;
                        }
                        else
                        {
                            // Non-GUID defaults (e.g. getdate(), numeric defaults):
                            // Retrieve via OUTPUT...INTO instead of a separate SELECT.
                            needOutputParam = true;
                        }

                        if (needOutputParam)
                        {
                            string sqlType = GetSqlTypeForOutput(col);
                            outputDeclare += outputComma + colName + " " + sqlType;
                            outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                            outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                            outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                            outputComma = ", ";
                            hasOutputCols = true;

                            clone = CloneParameter(p);
                            clone.Direction = ParameterDirection.Output;
                            if (col.CharacterMaxLength > 0)
                                clone.Size = (int)col.CharacterMaxLength;
                            cmd.Parameters.Add(clone);
                        }
                    }
                    else if (col.IsEntitySpacesConcurrency)
                    {
                        p = types[colName];
                        sql += " SET " + p.ParameterName + " = 1; ";

                        into += comma;
                        into += Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        values += comma;
                        values += "1";
                        comma = ", ";

                        // Capture the inserted concurrency value via OUTPUT...INTO
                        string sqlType = GetSqlTypeForOutput(col);
                        outputDeclare += outputComma + colName + " " + sqlType;
                        outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputComma = ", ";
                        hasOutputCols = true;

                        clone = CloneParameter(p);
                        clone.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(clone);
                    }
                    else if (col.IsAutoIncrement)
                    {
                        p = types[colName];
                        // Retrieve identity value via OUTPUT...INTO instead of SCOPE_IDENTITY()
                        string sqlType = GetSqlTypeForOutput(col);
                        outputDeclare += outputComma + colName + " " + sqlType;
                        outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputComma = ", ";
                        hasOutputCols = true;

                        clone = CloneParameter(p);
                        clone.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(clone);
                    }
                    else if (col.IsComputed || col.IsConcurrency)
                    {
                        // Computed/timestamp columns: echo back via OUTPUT...INTO
                        p = types[colName];

                        string sqlType = GetSqlTypeForOutput(col);
                        outputDeclare += outputComma + colName + " " + sqlType;
                        outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                        outputComma = ", ";
                        hasOutputCols = true;

                        clone = CloneParameter(p);
                        clone.Direction = ParameterDirection.Output;
                        if (col.CharacterMaxLength > 0)
                            clone.Size = (int)col.CharacterMaxLength;
                        cmd.Parameters.Add(clone);
                    }
                }
            }

            esColumnMetadataCollection cols = request.Columns;

            #region Special Column Logic (DateAdded, DateModified, AddedBy, ModifiedBy)
            if (cols.DateAdded != null && cols.DateAdded.IsServerSide)
            {
                p = CloneParameter(types[cols.DateAdded.ColumnName]);
                sql += " SET " + p.ParameterName + " = " + request.ProviderMetadata["DateAdded.ServerSideText"] + ";";
                CreateInsertSQLSnippet(cols.DateAdded.ColumnName, p, ref into, ref values, ref comma);

                string colName = cols.DateAdded.ColumnName;
                string sqlType = GetSqlTypeForOutput(cols.FindByColumnName(colName));
                outputDeclare += outputComma + colName + " " + sqlType;
                outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputComma = ", ";
                hasOutputCols = true;

                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
            }

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                p = CloneParameter(types[cols.DateModified.ColumnName]);
                sql += " SET " + p.ParameterName + " = " + request.ProviderMetadata["DateModified.ServerSideText"] + ";";
                CreateInsertSQLSnippet(cols.DateModified.ColumnName, p, ref into, ref values, ref comma);

                string colName = cols.DateModified.ColumnName;
                string sqlType = GetSqlTypeForOutput(cols.FindByColumnName(colName));
                outputDeclare += outputComma + colName + " " + sqlType;
                outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputComma = ", ";
                hasOutputCols = true;

                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
            }

            if (cols.AddedBy != null && cols.AddedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.AddedBy.ColumnName]);
                p.Size = (int)cols.FindByColumnName(cols.AddedBy.ColumnName).CharacterMaxLength;
                sql += " SET " + p.ParameterName + " = " + request.ProviderMetadata["AddedBy.ServerSideText"] + ";";
                CreateInsertSQLSnippet(cols.AddedBy.ColumnName, p, ref into, ref values, ref comma);

                string colName = cols.AddedBy.ColumnName;
                string sqlType = GetSqlTypeForOutput(cols.FindByColumnName(colName));
                outputDeclare += outputComma + colName + " " + sqlType;
                outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputComma = ", ";
                hasOutputCols = true;

                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.ModifiedBy.ColumnName]);
                p.Size = (int)cols.FindByColumnName(cols.ModifiedBy.ColumnName).CharacterMaxLength;
                sql += " SET " + p.ParameterName + " = " + request.ProviderMetadata["ModifiedBy.ServerSideText"] + ";";
                CreateInsertSQLSnippet(cols.ModifiedBy.ColumnName, p, ref into, ref values, ref comma);

                string colName = cols.ModifiedBy.ColumnName;
                string sqlType = GetSqlTypeForOutput(cols.FindByColumnName(colName));
                outputDeclare += outputComma + colName + " " + sqlType;
                outputCols += outputComma + "INSERTED." + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputInto += outputComma + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputSelect += outputComma + p.ParameterName + " = " + Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
                outputComma = ", ";
                hasOutputCols = true;

                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);
            }
            #endregion

            string fullName = CreateFullName(request);

            // Declare the OUTPUT table variable for all server-generated columns
            if (hasOutputCols)
            {
                sql += " DECLARE @output_vals TABLE (" + outputDeclare + ");";
            }

            sql += " INSERT INTO " + fullName + GetTableHints(packet) + " ";

            if (into.Length != 0 && hasOutputCols)
            {
                // Emit OUTPUT INSERTED...INTO before VALUES to capture all server-generated values
                sql += "(" + into + ") OUTPUT " + outputCols + " INTO @output_vals (" + outputInto + ") VALUES (" + values + ")";
            }
            else if (into.Length != 0)
            {
                // No output columns — plain INSERT
                sql += "(" + into + ") VALUES (" + values + ")";
            }
            else
            {
                // No explicit columns — DEFAULT VALUES
                sql += "DEFAULT VALUES";
            }

            // Guard against CHECK/FK constraint violations that silently fail on SQL Server 2025 Express
            sql += " IF @@ROWCOUNT = 0 RAISERROR('Insert failed: CHECK constraint violation or row was rejected.', 16, 1);";

            // Single SELECT to read back all server-generated column values from @output_vals
            if (hasOutputCols)
            {
                sql += " SELECT " + outputSelect + " FROM @output_vals;";
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        /// <summary>
        /// Returns the SQL Server type string needed for a TABLE variable column declaration
        /// used in the OUTPUT...INTO clause of BuildDynamicInsertCommand.
        /// Handles timestamp/rowversion specially since it cannot be declared in a TABLE variable
        /// as-is — it is mapped to binary(8) for storage purposes.
        /// </summary>
        private static string GetSqlTypeForOutput(esColumnMetadata col)
        {
            if (col.IsConcurrency)
                return "binary(8)";  // rowversion / timestamp cannot be declared in TABLE var directly

            switch (col.esType)
            {
                case esSystemType.Guid:       return "uniqueidentifier";
                case esSystemType.Boolean:    return "bit";
                case esSystemType.Byte:       return "tinyint";
                case esSystemType.Int16:      return "smallint";
                case esSystemType.Int32:      return "int";
                case esSystemType.Int64:      return "bigint";
                case esSystemType.Single:     return "real";
                case esSystemType.Double:     return "float";
                case esSystemType.Decimal:    return col.NumericPrecision > 0
                                                 ? "decimal(" + col.NumericPrecision + "," + col.NumericScale + ")"
                                                 : "decimal(18,4)";
                case esSystemType.DateTime:   return "datetime";
                case esSystemType.String:     return col.CharacterMaxLength > 0
                                                 ? "nvarchar(" + col.CharacterMaxLength + ")"
                                                 : "nvarchar(max)";
                default:                      return "sql_variant";
            }
        }

        static private void CreateInsertSQLSnippet(string colName, SqlParameter p, ref string into, ref string values, ref string comma)
        {
            into += comma;
            into += Delimiters.ColumnOpen + colName + Delimiters.ColumnClose;
            values += comma;
            values += p.ParameterName;
            comma = ", ";
        }

        static public SqlCommand BuildDynamicUpdateCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            string set = string.Empty;
            string sql = "SET NOCOUNT OFF; SET XACT_ABORT ON;"; // XACT_ABORT ensures constraint errors raise SqlException in all SQL Server versions
            sql += "UPDATE " + CreateFullName(request) + GetTableHints(packet) + " SET ";

            string where = String.Empty;
            string conncur = String.Empty;
            string computed = String.Empty;
            string comma = String.Empty;
            string and = String.Empty;
            string prolog = String.Empty;

            SqlParameter p = null;

            List<string> modifiedColumns = packet.ModifiedColumns;

            foreach (string colName in modifiedColumns)
            {
                esColumnMetadata col = request.Columns[colName];

                if (col == null) continue;

                if (!col.IsInPrimaryKey && !col.IsComputed)
                {
                    p = CloneParameter(types[colName]);
                    p = cmd.Parameters.Add(p);

                    object value = packet.CurrentValues[colName];
                    p.Value = value != null ? value : DBNull.Value;

                    sql += comma;
                    sql += Delimiters.ColumnOpen + colName + Delimiters.ColumnClose + " = " + p.ParameterName;
                    comma = ", ";
                }
            }

            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsInPrimaryKey)
                {
                    p = CloneParameter(types[col.Name]);
                    p.Value = packet.OriginalValues[col.Name];
                    cmd.Parameters.Add(p);

                    where += and;
                    where += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    and = " AND ";
                }
                else if (col.IsConcurrency)
                {
                    p = CloneParameter(types[col.Name]);
                    p.Value = packet.OriginalValues[col.Name];
                    p.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add(p);

                    int version = ResolveServerMajorVersion(request);

                    if (version >= 2008 || col.IsEntitySpacesConcurrency)
                        conncur += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    else
                        conncur += "TSEQUAL(" + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + "," + p.ParameterName + ")";

                    if (computed.Length > 0) computed += ", ";
                    computed += " " + p.ParameterName + " = " + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                }
                else if (col.IsComputed && !col.IsAutoIncrement)
                {
                    if (request.SelectedColumns != null && request.SelectedColumns.ContainsKey(col.Name))
                    {
                        p = CloneParameter(types[col.Name]);
                        p.Direction = ParameterDirection.Output;
                        if (col.CharacterMaxLength > 0)
                        {
                            p.Size = (int)col.CharacterMaxLength;
                        }
                        cmd.Parameters.Add(p);

                        if (computed.Length > 0) computed += ", ";
                        computed += " " + p.ParameterName + " = " + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    }
                }
                else if (col.IsEntitySpacesConcurrency)
                {
                    if (packet.OriginalValues != null && packet.OriginalValues.ContainsKey(col.Name))
                    {
                        p = CloneParameter(types[col.Name]);
                        p.Direction = ParameterDirection.InputOutput;
                        p.Value = packet.OriginalValues[col.Name];
                        cmd.Parameters.Add(p);

                        sql += comma.Length > 0 ? ", " : string.Empty;
                        sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " +
                            Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " + 1";

                        conncur += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;

                        prolog += " SET " + p.ParameterName + " = " + p.ParameterName + " + 1";
                    }
                }
            }

            esColumnMetadataCollection cols = request.Columns;

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                p = CloneParameter(types[cols.DateModified.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                sql += comma;
                sql += Delimiters.ColumnOpen + cols.DateModified.ColumnName + Delimiters.ColumnClose + " = " + p.ParameterName;
                comma = ", ";

                set += " SET " + p.ParameterName + " = " + request.ProviderMetadata["DateModified.ServerSideText"] + ";";
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.ModifiedBy.ColumnName]);
                p.Size = (int)cols.FindByColumnName(cols.ModifiedBy.ColumnName).CharacterMaxLength;
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                sql += comma;
                sql += Delimiters.ColumnOpen + cols.ModifiedBy.ColumnName + Delimiters.ColumnClose + " = " + p.ParameterName;
                comma = ", ";

                set += " SET " + p.ParameterName + " = " + request.ProviderMetadata["ModifiedBy.ServerSideText"] + ";";
            }


            sql = set + sql + " WHERE (" + where + ")";
            if (conncur.Length > 0)
            {
                sql += " AND " + conncur;
            }

            if (computed.Length > 0)
            {
                sql += " SELECT " + computed + " FROM " + CreateFullName(request) + " WHERE (" + where + ")";
            }

            if (prolog.Length > 0)
            {
                sql += prolog;
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        static public SqlCommand BuildDynamicDeleteCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            string sql = "SET NOCOUNT OFF; SET XACT_ABORT ON;"; // XACT_ABORT ensures constraint errors raise SqlException in all SQL Server versions
            sql += "DELETE FROM " + CreateFullName(request) + " ";

            string comma = String.Empty;
            string concur = String.Empty;
            comma = String.Empty;
            sql += " WHERE ";
            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsInPrimaryKey)
                {
                    SqlParameter p = CloneParameter(types[col.Name]);
                    p.Value = packet.OriginalValues[col.Name];
                    cmd.Parameters.Add(p);

                    sql += comma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    comma = " AND ";
                }
                else if (col.IsConcurrency || col.IsEntitySpacesConcurrency)
                {
                    SqlParameter p = CloneParameter(types[col.Name]);
                    p.Value = packet.OriginalValues[col.Name];
                    cmd.Parameters.Add(p);

                    int version = ResolveServerMajorVersion(request);

                    if (version >= 2008 || col.IsEntitySpacesConcurrency)
                        concur += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    else
                        concur += "TSEQUAL(" + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + "," + p.ParameterName + ")";
                }
            }

            if (concur.Length > 0)
            {
                sql += " AND " + concur;
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        static public SqlCommand BuildStoredProcInsertCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            if(request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = CreateFullSPName(request, request.ProviderMetadata.spInsert);

            PopulateStoredProcParameters(cmd, request, packet);

            esColumnMetadataCollection cols = request.Columns;

            foreach (esColumnMetadata col in cols)
            {
                if (col.HasDefault &&
                    (col.Default.ToLower().Contains("newid") || col.Default.ToLower().Contains("newsequentialid")))
                {
                    // They could pre-assign this even though it has a default
                    SqlParameter p = types[col.Name];
                    p = cmd.Parameters[p.ParameterName];

                    if (packet.ModifiedColumns.Contains(col.Name))
                    {
                        p.Direction = ParameterDirection.InputOutput;
                    }
                    else
                    {
                        p.Direction = ParameterDirection.Output;
                    }
                }
                else if (col.IsComputed || col.IsAutoIncrement || col.IsEntitySpacesConcurrency)
                {
                    SqlParameter p = types[col.Name];
                    p = cmd.Parameters[p.ParameterName];
                    p.Direction = ParameterDirection.Output;
                }
            }

            if (cols.DateAdded != null && cols.DateAdded.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.DateAdded.ColumnName].ParameterName];
                p = cmd.Parameters[p.ParameterName];
                p.Direction = ParameterDirection.Output;
            }

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.DateModified.ColumnName].ParameterName];
                p = cmd.Parameters[p.ParameterName];
                p.Direction = ParameterDirection.Output;
            }

            if (cols.AddedBy != null && cols.AddedBy.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.AddedBy.ColumnName].ParameterName];
                p.Size = (int)cols.FindByColumnName(cols.AddedBy.ColumnName).CharacterMaxLength;
                p = cmd.Parameters[p.ParameterName];
                p.Direction = ParameterDirection.Output;
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.ModifiedBy.ColumnName].ParameterName];
                p.Size = (int)cols.FindByColumnName(cols.ModifiedBy.ColumnName).CharacterMaxLength;
                p = cmd.Parameters[p.ParameterName];
                p.Direction = ParameterDirection.Output;
            }

            return cmd;
        }

        static public SqlCommand BuildStoredProcUpdateCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            if(request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = CreateFullSPName(request, request.ProviderMetadata.spUpdate);

            PopulateStoredProcParameters(cmd, request, packet);

            esColumnMetadataCollection cols = request.Columns;

            foreach (esColumnMetadata col in cols)
            {
                if (col.IsComputed || col.IsEntitySpacesConcurrency)
                {
                    SqlParameter p = types[col.Name];
                    p = cmd.Parameters[p.ParameterName];
                    p.Direction = ParameterDirection.InputOutput;
                }
            }

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.DateModified.ColumnName].ParameterName];
                p = cmd.Parameters[p.ParameterName];
                p.Value = null;
                p.Direction = ParameterDirection.Output;
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                SqlParameter p = cmd.Parameters[types[cols.ModifiedBy.ColumnName].ParameterName];
                p.Size = (int)cols.FindByColumnName(cols.ModifiedBy.ColumnName).CharacterMaxLength;
                p = cmd.Parameters[p.ParameterName];
                p.Value = null;
                p.Direction = ParameterDirection.Output;
            }

            return cmd;
        }

        static public SqlCommand BuildStoredProcDeleteCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlCommand cmd = new SqlCommand();
            if(request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = CreateFullSPName(request, request.ProviderMetadata.spDelete);

            SqlParameter p;

            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsInPrimaryKey)
                {
                    p = types[col.Name];
                    p = CloneParameter(p);
                    p.Value = packet.OriginalValues[col.Name];
                    cmd.Parameters.Add(p);
                }
                else if (col.IsConcurrency || col.IsEntitySpacesConcurrency)
                {
                    p = types[col.Name];
                    p = CloneParameter(p);
                    p.Value = packet.OriginalValues[col.Name];
                    cmd.Parameters.Add(p);
                }
            }

            return cmd;
        }

        static public void PopulateStoredProcParameters(SqlCommand cmd, esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, SqlParameter> types = Cache.GetParameters(request);

            SqlParameter p;

            foreach (esColumnMetadata col in request.Columns)
            {
                p = types[col.Name];
                p = CloneParameter(p);

                if (packet.CurrentValues.ContainsKey(col.Name))
                {
                    p.Value = packet.CurrentValues[col.Name];
                }

                if (p.SqlDbType == SqlDbType.Timestamp)
                {
                    p.Direction = ParameterDirection.InputOutput;
                }

                if (col.IsComputed && col.CharacterMaxLength > 0)
                {
                    p.Size = (int)col.CharacterMaxLength;
                }

                cmd.Parameters.Add(p);
            }
        }

        /// <summary>
        /// Resolves the SQL Server major version number to use for version-dependent SQL generation
        /// (e.g. rowversion comparison syntax).
        ///
        /// Priority:
        ///   1. request.DatabaseVersion if explicitly set by the caller (e.g. "2019", "15").
        ///   2. QueryBuilder.GetMajorVersion() — auto-detected from the server via SELECT @@VERSION,
        ///      cached per connection string so the round-trip happens only once per server.
        ///   3. 2012 as a safe default (covers all modern SQL Server features used here).
        ///
        /// Note: request.DatabaseVersion accepts either a 4-digit year ("2019") or the internal
        /// major version number ("15"). Both resolve correctly because the only branch in the
        /// callers is >= 2008, which is satisfied by any plausible value from either format.
        /// </summary>
        private static int ResolveServerMajorVersion(esDataRequest request)
        {
            // 1. Explicit override from caller
            if (!string.IsNullOrWhiteSpace(request.DatabaseVersion))
            {
                if (int.TryParse(request.DatabaseVersion, out int explicitVersion))
                    return explicitVersion;
            }

            // 2. Auto-detect from version cache if connection string is available
            if (!string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                int detected = QueryBuilder.GetMajorVersion(request.ConnectionString);
                if (detected > 0)
                    return detected;
            }

            // 3. Safe default — all SQL Server versions 2008+ support direct rowversion comparison
            return 2012;
        }

        static private SqlParameter CloneParameter(SqlParameter p)
        {
            ICloneable param = p as ICloneable;
            return param.Clone() as SqlParameter;
        }

        static public string CreateFullName(esDataRequest request, esDynamicQuery query)
        {
            IDynamicQueryInternal iQuery = query as IDynamicQueryInternal;

            esProviderSpecificMetadata providerMetadata = iQuery.ProviderMetadata as esProviderSpecificMetadata;

            string name = String.Empty;

            string catalog = iQuery.Catalog ?? request.Catalog ?? providerMetadata.Catalog;
            string schema = iQuery.Schema ?? request.Schema ?? providerMetadata.Schema;

            if (!string.IsNullOrWhiteSpace(catalog) && !string.IsNullOrWhiteSpace(schema))
            {
                name += Delimiters.TableOpen + catalog + Delimiters.TableClose + ".";
            }

            if (!string.IsNullOrWhiteSpace(schema))
            {
                name += Delimiters.TableOpen + schema + Delimiters.TableClose + ".";
            }

            name += Delimiters.TableOpen;

            if (query.querySource != null)
                name += query.querySource;
            else
                name += providerMetadata.Destination;
            name += Delimiters.TableClose;

            return name;
        }

        static public string GetTableHints(esEntitySavePacket packet)
        {
            if (string.IsNullOrWhiteSpace(packet.TableHints))
            {
                return string.Empty;
            }
            return " with (" + packet.TableHints.Trim() + ")";
        }

        /// <summary>
        /// Builds the fully qualified table name using catalog, schema and table identifiers.
        /// </summary>
        static public string CreateFullName(esDataRequest request)
        {
            string name = String.Empty;

            string catalog = request.Catalog ?? request.ProviderMetadata.Catalog;
            string schema = request.Schema ?? request.ProviderMetadata.Schema;

            if (!string.IsNullOrWhiteSpace(catalog) && !string.IsNullOrWhiteSpace(schema))
            {
                name += Delimiters.TableOpen + catalog + Delimiters.TableClose + ".";
            }

            if (!string.IsNullOrWhiteSpace(schema))
            {
                name += Delimiters.TableOpen + schema + Delimiters.TableClose + ".";
            }

            name += Delimiters.TableOpen;

            if (request.DynamicQuery != null && request.DynamicQuery.querySource != null)
                name += request.DynamicQuery.querySource;
            else
                name += request.QueryText != null ? request.QueryText : request.ProviderMetadata.Destination;
            name += Delimiters.TableClose;

            return name;
        }

        static public string CreateFullSPName(esDataRequest request, string spName)
        {
            string name = String.Empty;

            if ( (!string.IsNullOrWhiteSpace(request.Catalog) || !string.IsNullOrWhiteSpace(request.ProviderMetadata.Catalog)) &&
                 (!string.IsNullOrWhiteSpace(request.Schema) || !string.IsNullOrWhiteSpace(request.ProviderMetadata.Schema)) )
            {
                name += Delimiters.TableOpen;
                name += !string.IsNullOrWhiteSpace(request.Catalog) ? request.Catalog : request.ProviderMetadata.Catalog;
                name += Delimiters.TableClose + ".";
            }

            if (!string.IsNullOrWhiteSpace(request.Schema) || !string.IsNullOrWhiteSpace(request.ProviderMetadata.Schema))
            {
                name += Delimiters.TableOpen;
                name += !string.IsNullOrWhiteSpace(request.Schema) ? request.Schema : request.ProviderMetadata.Schema;
                name += Delimiters.TableClose + ".";
            }

            name += Delimiters.StoredProcNameOpen;
            name += spName;
            name += Delimiters.StoredProcNameClose;

            return name;
        }

        static public esConcurrencyException CheckForConcurrencyException(SqlException ex)
        {
            esConcurrencyException ce = null;

            if (ex.Errors != null)
            {
                foreach (SqlError err in ex.Errors)
                {
                    // 532  = timestamp/rowversion mismatch (legacy SQL Server concurrency)
                    // 2601 = unique index violation (duplicate key on unique index)
                    // 2627 = primary key / unique constraint violation
                    // 1205 = deadlock victim
                    // 1222 = lock request timeout (lock_timeout exceeded)
                    if (err.Number == 532 || err.Number == 2601 ||
                        err.Number == 2627 || err.Number == 1205 || err.Number == 1222)
                    {
                        ce = new esConcurrencyException(err.Message, ex);
                        ce.Source = err.Source;
                        break;
                    }
                }
            }

            return ce;
        }

        static public void AddParameters(SqlCommand cmd, esDataRequest request)
        {
            if (request.QueryType == esQueryType.Text && request.QueryText != null && request.QueryText.Contains("{0}"))
            {
                int i = 0;
                string token = String.Empty;
                string sIndex = String.Empty;
                string param = String.Empty;

                foreach (esParameter esParam in request.Parameters)
                {
                    sIndex = i.ToString();
                    token = '{' + sIndex + '}';
                    param = Delimiters.Param + "p" + sIndex;
                    request.QueryText = request.QueryText.Replace(token, param);
                    i++;

                    SqlParameter p = cmd.Parameters.AddWithValue(Delimiters.Param + esParam.Name, esParam.Value);

                    if (esParam.UdtTypeName != null)
                    {
                        p.UdtTypeName = esParam.UdtTypeName;
                    }
                }
            }
            else
            {
                SqlParameter param;

                foreach (esParameter esParam in request.Parameters)
                {
                    param = cmd.Parameters.AddWithValue(Delimiters.Param + esParam.Name, esParam.Value);

                    switch (esParam.Direction)
                    {
                        case esParameterDirection.InputOutput:
                            param.Direction = ParameterDirection.InputOutput;
                            break;

                        case esParameterDirection.Output:
                            param.Direction = ParameterDirection.Output;
                            param.DbType = esParam.DbType;
                            param.Size = esParam.Size;
                            param.Scale = esParam.Scale;
                            param.Precision = esParam.Precision;
                            break;

                        case esParameterDirection.ReturnValue:
                            param.Direction = ParameterDirection.ReturnValue;
                            break;

                        // The default is ParameterDirection.Input;
                    }

                    if (esParam.UdtTypeName != null)
                    {
                        param.UdtTypeName = esParam.UdtTypeName;
                    }
                }
            }
        }

        static public void GatherReturnParameters(SqlCommand cmd, esDataRequest request, esDataResponse response)
        {
            if (cmd.Parameters.Count > 0)
            {
                if (request.Parameters != null && request.Parameters.Count > 0)
                {
                    response.Parameters = new esParameters();

                    foreach (esParameter esParam in request.Parameters)
                    {
                        if (esParam.Direction != esParameterDirection.Input)
                        {
                            response.Parameters.Add(esParam);
                            SqlParameter p = cmd.Parameters[Delimiters.Param + esParam.Name];
                            esParam.Value = p.Value;
                        }
                    }
                }
            }
        }
    }
}

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

using EntitySpaces.DynamicQuery;
using EntitySpaces.Interfaces;

using Npgsql;
using NpgsqlTypes;

namespace EntitySpaces.Npgsql2Provider
{
    class Shared
    {

        // ===================================================================
        // Builds an INSERT command for a single entity.
        //
        // [NEW ADJUSTMENT] Key changes vs. the previous version:
        //   1. When an auto-increment PK is supplied explicitly by the user,
        //      the column is sent as InputOutput and added to RETURNING, so the
        //      framework receives the value back and the entity is marked clean.
        //   2. When a non-auto-increment PK is not covered by the isModified
        //      branch, the PK is added to RETURNING only if it is auto-increment.
        //   3. [BUGFIX] DateModified / AddedBy / ModifiedBy special columns now
        //      add their own column name to RETURNING (previously all three
        //      incorrectly returned "DateAdded").
        //   4. [BUGFIX] AddedBy no longer dereferences cols.ModifiedBy when
        //      reading the CharacterMaxLength — it uses cols.AddedBy.
        //   5. Dead local variables (where, autoInc) removed.
        //   6. [FIX1] Explicit-PK detection now uses IsColumnModified so that a
        //      user-supplied PK reaches us under either the column name or the
        //      property name. Fixes Categories_Can_Insert_With_Explicit_PK on
        //      snake_case providers.
        // ===================================================================
        static public NpgsqlCommand BuildDynamicInsertCommand(esDataRequest request, esEntitySavePacket packet)
        {
            string sql = String.Empty;
            string into = String.Empty;
            string values = String.Empty;
            string comma = String.Empty;
            List<string> returningCols = new List<string>();

            NpgsqlParameter p = null;

            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            esColumnMetadataCollection cols = request.Columns;
            foreach (esColumnMetadata col in cols)
            {
                // [FIX-BUG-22] Skip special columns BEFORE any other classification.
                // The Studio metadata engine marks timestamp-based special columns
                // (DateAdded / DateModified) as IsConcurrency = true. Without this
                // guard they fall into the IsConcurrency branch below, are added as
                // InputOutput with no Value, and Npgsql rejects the command with
                // "Parameter ':DateAdded' cannot be null".
                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                // [NEW ADJUSTMENT] Use the tolerant helpers so that property-name-keyed
                // modifications coming from generated ApplyPostSaveKeys are recognized.
                bool isModified = IsColumnModified(packet, col);

                if (isModified && (!col.IsAutoIncrement && !col.IsConcurrency && !col.IsEntitySpacesConcurrency))
                {
                    p = cmd.Parameters.Add(CloneParameter(types[col.Name]));

                    object value = GetColumnValue(packet, col);          // [NEW ADJUSTMENT]
                    p.Value = value != null ? value : DBNull.Value;

                    into += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    values += comma + p.ParameterName;
                    comma = ", ";
                }
                else if (col.IsAutoIncrement)
                {
                    // [FIX1] Tolerant check — the user may have supplied the PK
                    // under either the DB column name or the property name. Both
                    // are valid markers of an explicit value.
                    bool hasExplicitValue = IsColumnModified(packet, col);

                    if (hasExplicitValue)
                    {
                        // [NEW ADJUSTMENT] The user assigned an explicit value to an
                        // auto-increment column. Send it as InputOutput and include
                        // the column in RETURNING so the framework receives the
                        // value back and can mark the entity as clean.
                        p = cmd.Parameters.Add(CloneParameter(types[col.Name]));
                        object value = GetColumnValue(packet, col);              // [NEW ADJUSTMENT]
                        p.Value = value != null ? value : DBNull.Value;
                        p.Direction = ParameterDirection.InputOutput;   // [NEW ADJUSTMENT] was: Input (default)

                        into += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                        values += comma + p.ParameterName;
                        comma = ", ";

                        // [NEW ADJUSTMENT] Always include the explicit PK in RETURNING.
                        returningCols.Add(Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose);
                    }
                    else
                    {
                        // No explicit value — let the sequence generate the ID
                        returningCols.Add(Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose);
                        p = CloneParameter(types[col.Name]);
                        p.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(p);
                    }
                }
                else if (col.IsConcurrency)
                {
                    // These columns have defaults and they weren't supplied with values,
                    // so let's return them
                    p = cmd.Parameters.Add(CloneParameter(types[col.Name]));
                    p.Direction = ParameterDirection.InputOutput;

                    returningCols.Add(Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose);

                    if (col.CharacterMaxLength > 0)
                    {
                        p.Size = (int)col.CharacterMaxLength;
                    }
                }
                else if (col.IsEntitySpacesConcurrency)
                {
                    p = cmd.Parameters.Add(CloneParameter(types[col.Name]));
                    p.Direction = ParameterDirection.Output;

                    into += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    values += comma + "1";
                    comma = ", ";

                    p.Value = 1; // Seems to work, We'll take it ...
                }
                else if (col.IsComputed)
                {
                    // Do nothing but leave this here
                }
                else if (cols.IsSpecialColumn(col))
                {
                    // Do nothing but leave this here
                }
                else if (col.HasDefault)
                {
                    // These columns have defaults and they weren't supplied with values,
                    // so let's return them
                    p = cmd.Parameters.Add(CloneParameter(types[col.Name]));
                    p.Direction = ParameterDirection.InputOutput;
                    p.Value = DBNull.Value; // required by Npgsql — null is not valid, use DBNull.Value

                    // Add to RETURNING instead of second SELECT
                    returningCols.Add(Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose);

                    if (col.CharacterMaxLength > 0)
                    {
                        p.Size = (int)col.CharacterMaxLength;
                    }
                }

                // [NEW ADJUSTMENT] PK fallback block. Reached only when the PK
                // column was not already added by the branches above. Explicit
                // auto-increment values are handled by the IsAutoIncrement branch;
                // manual PKs supplied as ModifiedColumns are handled by the
                // isModified branch. This path covers PK columns present in the
                // packet's OriginalValues but not in CurrentValues (e.g. composite
                // PKs where only part of the key was supplied).
                if (col.IsInPrimaryKey)
                {
                    NpgsqlParameter typeParam = types[col.Name];

                    if (!cmd.Parameters.Contains(typeParam.ParameterName))
                    {
                        p = CloneParameter(typeParam);

                        if (col.IsAutoIncrement && !isModifiedPK(packet, col))
                        {
                            // Sequence-generated PK — output only, value comes from RETURNING
                            p.Direction = ParameterDirection.Output;

                            // [NEW ADJUSTMENT] Ensure the auto-increment PK is also
                            // present in RETURNING, since the value is expected back
                            // on the client. Guard against duplicates.
                            string colToken = Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                            if (!returningCols.Contains(colToken))
                            {
                                returningCols.Add(colToken);
                            }
                        }
                        else
                        {
                            // Manual PK or explicit value — preserve the supplied value
                            p.Direction = ParameterDirection.Input;
                            object value = GetColumnValue(packet, col);              // [NEW ADJUSTMENT]
                            p.Value = value != null ? value : DBNull.Value;
                        }

                        cmd.Parameters.Add(p);
                    }
                }
            }

            #region Special Column Logic
            // [FIX-BUG-19] The ProviderMetadata ServerSideText for these columns is
            // configured as the column name itself ("DateAdded"), which is not a
            // valid SQL expression. Concatenating it into the VALUES clause produces
            // bareword identifiers that PostgreSQL resolves to NULL on the new row,
            // tripping the NOT NULL constraint.
            //
            // The correct behaviour for a ServerSide special column with a DB-level
            // DEFAULT is to omit it from the INSERT column list and VALUES list, and
            // rely on the DEFAULT to populate the value. RETURNING fetches it back.
            if (cols.DateAdded != null && cols.DateAdded.IsServerSide)
            {
                p = CloneParameter(types[cols.DateAdded.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                returningCols.Add(Delimiters.ColumnOpen + cols.DateAdded.ColumnName + Delimiters.ColumnClose);
            }

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                p = CloneParameter(types[cols.DateModified.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                returningCols.Add(Delimiters.ColumnOpen + cols.DateModified.ColumnName + Delimiters.ColumnClose);
            }

            if (cols.AddedBy != null && cols.AddedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.AddedBy.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                returningCols.Add(Delimiters.ColumnOpen + cols.AddedBy.ColumnName + Delimiters.ColumnClose);

                esColumnMetadata col = request.Columns[cols.AddedBy.ColumnName];
                if (col.CharacterMaxLength > 0)
                    p.Size = (int)col.CharacterMaxLength;
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.ModifiedBy.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                returningCols.Add(Delimiters.ColumnOpen + cols.ModifiedBy.ColumnName + Delimiters.ColumnClose);

                esColumnMetadata col = request.Columns[cols.ModifiedBy.ColumnName];
                if (col.CharacterMaxLength > 0)
                    p.Size = (int)col.CharacterMaxLength;
            }
            #endregion

            string fullName = CreateFullName(request);

            sql += " INSERT INTO " + fullName;

            if (into.Length != 0)
            {
                sql += " (" + into + ") VALUES (" + values + ")";
            }
            else
            {
                sql += " DEFAULT VALUES";
            }

            // Single RETURNING clause replaces both the old returning string and
            // the second SELECT. All identity columns, defaults, concurrency, and
            // special columns are returned together.
            if (returningCols.Count > 0)
            {
                sql += " RETURNING " + string.Join(", ", returningCols);
            }

            sql += ";";

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        // ===================================================================
        // [FIX1] Reuses the tolerant column-modified check so a user-supplied
        // PK recognized under the property name ("CategoryId") is treated the
        // same as one recognized under the column name ("category_id").
        // ===================================================================
        private static bool isModifiedPK(esEntitySavePacket packet, esColumnMetadata col)
        {
            return IsColumnModified(packet, col);
        }

        // ===================================================================
        // Builds an UPDATE command for a single entity.
        //
        // [NEW ADJUSTMENT] Key change vs. the previous version:
        //   Modified-column detection and value lookup now accept both the DB
        //   column name ("order_id") and the EntitySpaces property name
        //   ("OrderId"). Generated ApplyPostSaveKeys calls SetProperty with the
        //   property name, which previously fell through silently on providers
        //   that use snake_case column names (PostgreSQL, Oracle, ...). On SQL
        //   Server both names coincide, so behaviour is unchanged there.
        // ===================================================================
        static public NpgsqlCommand BuildDynamicUpdateCommand(esDataRequest request, esEntitySavePacket packet)
        {
            string where = String.Empty;
            string conncur = String.Empty;
            string scomma = String.Empty;
            string defaults = String.Empty;
            string defaultsComma = String.Empty;
            string and = String.Empty;

            string sql = "UPDATE " + CreateFullName(request) + " SET ";

            PropertyCollection props = new PropertyCollection();
            NpgsqlParameter p = null;

            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            esColumnMetadataCollection cols = request.Columns;
            foreach (esColumnMetadata col in cols)
            {
                // [FIX-BUG-22] Skip special columns first — same rationale as INSERT.
                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                // [NEW ADJUSTMENT] Tolerant modified-column check.
                bool isModified = IsColumnModified(packet, col);

                if (isModified && (!col.IsAutoIncrement && !col.IsConcurrency && !col.IsEntitySpacesConcurrency))
                {
                    p = cmd.Parameters.Add(CloneParameter(types[col.Name]));

                    // [NEW ADJUSTMENT] Tolerant value lookup.
                    object value = GetColumnValue(packet, col);
                    p.Value = value != null ? value : DBNull.Value;

                    sql += scomma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    scomma = ", ";
                }
                else if (col.IsAutoIncrement)
                {
                    // Nothing to do but leave this here
                }
                else if (col.IsConcurrency)
                {
                    p = CloneParameter(types[col.Name]);
                    p.SourceVersion = DataRowVersion.Original;
                    p.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add(p);

                    conncur += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                }
                else if (col.IsEntitySpacesConcurrency)
                {
                    p = CloneParameter(types[col.Name]);

                    // [NEW ADJUSTMENT] Tolerant original-value lookup.
                    p.Value = GetOriginalColumnValue(packet, col);
                    p.Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.Add(p);

                    sql += scomma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName + " + 1";

                    conncur += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;

                    defaults += defaultsComma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    defaultsComma = ",";
                }
                else if (col.IsComputed)
                {
                    // Do nothing but leave this here
                }
                else if (cols.IsSpecialColumn(col))
                {
                    // Do nothing but leave this here
                }
                else if (col.HasDefault)
                {
                    // defaults += defaultsComma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    // defaultsComma = ",";
                }

                if (col.IsInPrimaryKey)
                {
                    p = CloneParameter(types[col.Name]);

                    // [NEW ADJUSTMENT] Tolerant original-value lookup for the WHERE clause.
                    p.Value = GetOriginalColumnValue(packet, col);
                    cmd.Parameters.Add(p);

                    where += and + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    and = " AND ";
                }
            }

            #region Special Column Logic
            // [FIX-BUG-23] A server-side special column cannot be expressed as a raw
            // SQL fragment from ServerSideText: the Studio-generated config emits the
            // column name itself ("DateModified"), producing
            // SET "DateModified" = DateModified which PostgreSQL folds to lowercase
            // and rejects with "column datemodified does not exist".
            //
            // The correct handling for a server-side timestamp is a DB trigger (or an
            // application-level SetColumn before Save). This block only registers the
            // parameter as Output and requests the value back through the trailing
            // SELECT so the framework refreshes the entity with the current value.
            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                p = CloneParameter(types[cols.DateModified.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                defaults += defaultsComma + Delimiters.ColumnOpen + cols.DateModified.ColumnName + Delimiters.ColumnClose;
                defaultsComma = ",";
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                p = CloneParameter(types[cols.ModifiedBy.ColumnName]);
                p.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(p);

                defaults += defaultsComma + Delimiters.ColumnOpen + cols.ModifiedBy.ColumnName + Delimiters.ColumnClose;
                defaultsComma = ",";

                esColumnMetadata col = request.Columns[cols.ModifiedBy.ColumnName];
                if (col.CharacterMaxLength > 0)
                    p.Size = (int)col.CharacterMaxLength;
            }
            #endregion

            sql += " WHERE " + where + "";
            if (conncur.Length > 0)
            {
                sql += " AND " + conncur;
            }

            if (defaults.Length > 0)
            {
                sql += "; SELECT " + defaults + " FROM " + CreateFullName(request) + " WHERE (" + where + ")";
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        // ===================================================================
        // Builds a DELETE command for a single entity.
        //
        // [NEW ADJUSTMENT] Key change vs. the previous version:
        //   Original-value lookup for PK and concurrency columns now accepts
        //   both the DB column name ("order_id") and the EntitySpaces property
        //   name ("OrderId"). Generated code may store the key under either
        //   depending on how the entity was populated before MarkAsDeleted().
        // ===================================================================
        static public NpgsqlCommand BuildDynamicDeleteCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            string sql = "DELETE FROM " + CreateFullName(request) + " ";

            string comma = String.Empty;
            comma = String.Empty;
            sql += " WHERE ";
            foreach (esColumnMetadata col in request.Columns)
            {
                // [FIX-BUG-22] Skip special columns first.
                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                if (col.IsInPrimaryKey || col.IsEntitySpacesConcurrency)
                {
                    NpgsqlParameter p = types[col.Name];
                    p = cmd.Parameters.Add(CloneParameter(p));

                    // [NEW ADJUSTMENT] Tolerant original-value lookup.
                    p.Value = GetOriginalColumnValue(packet, col);

                    sql += comma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    comma = " AND ";
                }
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        // ===================================================================
        // Builds a SELECT * FROM "proc_insert"(:args) command for the
        // StoredProcedure INSERT path.
        //
        // [FIX-BUG-14] PostgreSQL functions are invoked via SELECT * FROM
        // "func"(:args), not via CommandType.StoredProcedure (which Npgsql
        // translates to CALL, valid only for PROCEDURES). The function
        // RETURNS SETOF "<table>", so the resulting rowset carries the
        // inserted row and is read back with ExecuteReader.
        //
        // Args are positional in the same order the template iterates the
        // columns (all non-auto, non-computed columns).
        // ===================================================================
        static public NpgsqlCommand BuildStoredProcInsertCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
            cmd.CommandType = CommandType.Text;

            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT * FROM ")
              .Append(Delimiters.StoredProcNameOpen)
              .Append(request.ProviderMetadata.spInsert)
              .Append(Delimiters.StoredProcNameClose)
              .Append("(");

            bool first = true;
            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsAutoIncrement || col.IsComputed) continue;
                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                NpgsqlParameter p = CloneParameter(types[col.Name]);

                // [FIX-BUG-22] EntitySpaces concurrency columns start at 1 on insert.
                // Matches the DynamicSQL path, which hardcodes the literal 1 for
                // IsEntitySpacesConcurrency columns. The framework does not populate
                // CurrentValues["Version"] before SaveToProvider, so without this
                // default the parameter binds as DBNull and PostgreSQL rejects the
                // NOT NULL insert with SQLSTATE 23502.
                object value;
                if (col.IsEntitySpacesConcurrency)
                {
                    value = 1;
                }
                else
                {
                    value = GetColumnValue(packet, col);
                }

                p.Value = value ?? (object)DBNull.Value;
                p.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p);

                if (!first) sb.Append(", ");
                sb.Append(p.ParameterName);
                first = false;
            }

            sb.Append(")");
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        // ===================================================================
        // [FIX-BUG-14] SELECT * FROM "proc_update"(:args) — same pattern as
        // the INSERT path. Args include all non-computed columns (the SP
        // expects the full row so it can UPDATE and RETURN the new state).
        // ===================================================================
        static public NpgsqlCommand BuildStoredProcUpdateCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
            cmd.CommandType = CommandType.Text;

            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT * FROM ")
              .Append(Delimiters.StoredProcNameOpen)
              .Append(request.ProviderMetadata.spUpdate)
              .Append(Delimiters.StoredProcNameClose)
              .Append("(");

            bool first = true;
            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsComputed) continue;

                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                NpgsqlParameter p = CloneParameter(types[col.Name]);
                object value = col.IsInPrimaryKey
                    ? GetOriginalColumnValue(packet, col)
                    : GetColumnValue(packet, col);
                p.Value = value ?? (object)DBNull.Value;
                p.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p);

                if (!first) sb.Append(", ");
                sb.Append(p.ParameterName);
                first = false;
            }

            sb.Append(")");
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        // ===================================================================
        // [FIX-BUG-14] SELECT "proc_delete"(:pk-args) — the function returns
        // void. Only PK and concurrency columns are passed.
        // ===================================================================
        static public NpgsqlCommand BuildStoredProcDeleteCommand(esDataRequest request, esEntitySavePacket packet)
        {
            Dictionary<string, NpgsqlParameter> types = Cache.GetParameters(request);

            NpgsqlCommand cmd = new NpgsqlCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
            cmd.CommandType = CommandType.Text;

            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ")
              .Append(Delimiters.StoredProcNameOpen)
              .Append(request.ProviderMetadata.spDelete)
              .Append(Delimiters.StoredProcNameClose)
              .Append("(");

            bool first = true;
            foreach (esColumnMetadata col in request.Columns)
            {
                // [V3-FIX] DELETE takes only PK columns. Matches the template.
                if (!col.IsInPrimaryKey) continue;

                if (IsSpecialColumnSafe(request.Columns, col)) continue;

                NpgsqlParameter p = CloneParameter(types[col.Name]);
                p.Value = GetOriginalColumnValue(packet, col) ?? (object)DBNull.Value;
                p.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p);

                if (!first) sb.Append(", ");
                sb.Append(p.ParameterName);
                first = false;
            }

            sb.Append(")");
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        static private NpgsqlParameter CloneParameter(NpgsqlParameter p)
        {
            ICloneable param = p as ICloneable;
            return param.Clone() as NpgsqlParameter;
        }

        static public string CreateFullName(esDataRequest request, esDynamicQuery query)
        {
            IDynamicQueryInternal iQuery = query as IDynamicQueryInternal;

            esProviderSpecificMetadata providerMetadata = iQuery.ProviderMetadata as esProviderSpecificMetadata;

            string name = String.Empty;

            string catalog = iQuery.Catalog ?? request.Catalog ?? providerMetadata.Catalog;
            string schema = iQuery.Schema ?? request.Schema ?? providerMetadata.Schema;

            if (catalog != null && schema != null)
            {
                name += Delimiters.TableOpen + catalog + Delimiters.TableClose + ".";
            }

            if (schema != null)
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

        static public string CreateFullName(esDataRequest request)
        {
            string name = String.Empty;

            string catalog = request.Catalog ?? request.ProviderMetadata.Catalog;
            string schema = request.Schema ?? request.ProviderMetadata.Schema;

            if (catalog != null && schema != null)
            {
                name += Delimiters.TableOpen + catalog + Delimiters.TableClose + ".";
            }

            if (schema != null)
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

            if ((request.Catalog != null || request.ProviderMetadata.Catalog != null) &&
                 (request.Schema != null || request.ProviderMetadata.Schema != null))
            {
                name += Delimiters.TableOpen;
                name += request.Catalog != null ? request.Catalog : request.ProviderMetadata.Catalog;
                name += Delimiters.TableClose + ".";
            }

            if (request.Schema != null || request.ProviderMetadata.Schema != null)
            {
                name += Delimiters.TableOpen;
                name += request.Schema != null ? request.Schema : request.ProviderMetadata.Schema;
                name += Delimiters.TableClose + ".";
            }

            name += Delimiters.StoredProcNameOpen;
            name += spName;
            name += Delimiters.StoredProcNameClose;

            return name;
        }

        // Development tip: add 'Include Error Detail=true' to the connection string
        // to get full PostgreSQL error details in exception messages.
        // Example: "Host=...;Database=...;Include Error Detail=true"
        // WARNING: do not use in production — may expose sensitive data in error messages.
        static public esConcurrencyException CheckForConcurrencyException(NpgsqlException ex)
        {
            esConcurrencyException ce = null;

            // SqlState via reflection — compatible with net48 and Npgsql legacy
            string sqlState = null;

            try
            {
                var prop = ex.GetType().GetProperty("SqlState")
                        ?? ex.GetType().GetProperty("Code");

                if (prop != null)
                    sqlState = prop.GetValue(ex) as string;
            }
            catch { /* ignore reflection errors */ }

            switch (sqlState)
            {
                case "23505": // unique_violation — duplicate key
                case "40001": // serialization_failure
                case "40P01": // deadlock_detected
                case "55P03": // lock_not_available
                    ce = new esConcurrencyException(ex.Message, ex);
                    break;
            }


            return ce;
        }

        static public void AddParameters(NpgsqlCommand cmd, esDataRequest request)
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

                    cmd.Parameters.AddWithValue(Delimiters.Param + esParam.Name, esParam.Value);
                }
            }
            else
            {
                NpgsqlParameter param;

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
                }
            }
        }

        static public void GatherReturnParameters(NpgsqlCommand cmd, esDataRequest request, esDataResponse response)
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
                            NpgsqlParameter p = cmd.Parameters[Delimiters.Param + esParam.Name];
                            esParam.Value = p.Value;
                        }
                    }
                }
            }
        }


        // ===================================================================
        // [NEW ADJUSTMENT] Checks if a column is marked as modified accepting
        // both the DB column name and the EntitySpaces property name. This
        // accommodates the generated code which, for providers that use
        // snake_case column names (PostgreSQL, Oracle, ...), calls
        // SetProperty("OrderId", value) with the property name instead of
        // the column name ("order_id"). On SQL Server both names coincide
        // so this is a no-op there.
        // ===================================================================
        static private bool IsColumnModified(esEntitySavePacket packet, esColumnMetadata col)
        {
            if (packet.ModifiedColumns == null) return false;
            if (packet.ModifiedColumns.Contains(col.Name)) return true;
            if (!string.IsNullOrEmpty(col.PropertyName) &&
                packet.ModifiedColumns.Contains(col.PropertyName)) return true;
            return false;
        }

        // ===================================================================
        // [NEW ADJUSTMENT - REVISED] Reads a column value from CurrentValues,
        // accepting either the DB column name or the EntitySpaces property name.
        //
        // IMPORTANT: esSmartDictionary exposes all registered column names via
        // ContainsKey even when no value has been assigned (they resolve to
        // null/DBNull). Checking ContainsKey alone is therefore insufficient —
        // we must check the VALUE, and only fall through to the property-name
        // lookup when the column-name slot is empty.
        // ===================================================================
        static private object GetColumnValue(esEntitySavePacket packet, esColumnMetadata col)
        {
            object byColumn = null;
            if (packet.CurrentValues.ContainsKey(col.Name))
                byColumn = packet.CurrentValues[col.Name];

            // Only use the column-name slot if it holds a real value.
            if (byColumn != null && byColumn != DBNull.Value)
                return byColumn;

            // Fall back to the property-name slot (populated by SetProperty from
            // generated ApplyPostSaveKeys on snake_case providers).
            if (!string.IsNullOrEmpty(col.PropertyName) &&
                packet.CurrentValues.ContainsKey(col.PropertyName))
            {
                object byProperty = packet.CurrentValues[col.PropertyName];
                if (byProperty != null && byProperty != DBNull.Value)
                    return byProperty;
            }

            // Neither slot holds a value — return whatever we have so the caller
            // can bind NULL / DBNull to the parameter.
            return byColumn;
        }

        // ===================================================================
        // [NEW ADJUSTMENT - REVISED] Same fix as GetColumnValue, applied to
        // OriginalValues. Used by UPDATE and DELETE to build the WHERE clause.
        // ===================================================================
        static private object GetOriginalColumnValue(esEntitySavePacket packet, esColumnMetadata col)
        {
            if (packet.OriginalValues == null) return null;

            object byColumn = null;
            if (packet.OriginalValues.ContainsKey(col.Name))
                byColumn = packet.OriginalValues[col.Name];

            if (byColumn != null && byColumn != DBNull.Value)
                return byColumn;

            if (!string.IsNullOrEmpty(col.PropertyName) &&
                packet.OriginalValues.ContainsKey(col.PropertyName))
            {
                object byProperty = packet.OriginalValues[col.PropertyName];
                if (byProperty != null && byProperty != DBNull.Value)
                    return byProperty;
            }

            return byColumn;
        }

        // [FIX-BUG-22] Defense-in-depth: the metadata IsSpecialColumn relies on
        // the runtime esColumnMetadataCollection.DateAdded field being populated.
        // In some code paths (SP collection save) the flags appear unset and the
        // check falls through, so special columns end up being sent as normal
        // parameters — DateAdded / DateModified as null, which Npgsql rejects.
        // This local helper matches by name to guarantee exclusion.
        static private bool IsSpecialColumnSafe(esColumnMetadataCollection columns, esColumnMetadata col)
        {
            if (col == null || string.IsNullOrEmpty(col.Name)) return false;

            // Try the metadata-driven check first.
            try { if (columns.IsSpecialColumn(col)) return true; } catch { }

            // Fallback: match by standard name.
            string n = col.Name;
            if (string.Equals(n, "DateAdded", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(n, "DateModified", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(n, "AddedBy", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(n, "ModifiedBy", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

    } // end class
}

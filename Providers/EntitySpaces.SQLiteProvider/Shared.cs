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
using System.Data.SQLite;

using EntitySpaces.DynamicQuery;
using EntitySpaces.Interfaces;

namespace EntitySpaces.SQLiteProvider
{
    class Shared
    {
        static public SQLiteCommand BuildDynamicInsertCommand(esDataRequest request, esEntitySavePacket packet)
        {
            string sql = String.Empty;
            string defaults = String.Empty;
            string into = String.Empty;
            string values = String.Empty;
            string comma = String.Empty;
            string defaultComma = String.Empty;
            string where = String.Empty;
            string whereComma = String.Empty;

            PropertyCollection props = new PropertyCollection();

            Dictionary<string, SQLiteParameter> types = Cache.GetParameters(request);

            SQLiteCommand cmd = new SQLiteCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            esColumnMetadataCollection cols = request.Columns;
            foreach (esColumnMetadata col in cols)
            {
                bool isModified = IsColumnModified(packet, col);

                if (isModified && (!col.IsAutoIncrement && !col.IsConcurrency && !col.IsEntitySpacesConcurrency))
                {
                    SQLiteParameter p = types[col.Name];
                    SQLiteParameter clone = CloneParameter(p, col);
                    cmd.Parameters.Add(clone);  

                    into += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    values += comma + p.ParameterName;
                    comma = ", ";
                }
                else if (col.IsAutoIncrement)
                {
                    // Check whether the caller supplied an explicit PK value.
                    // SQLite rowid tables accept an explicit integer PK — in that case we
                    // include the column in the INSERT and skip the last_insert_rowid() fetch,
                    // because the row was inserted with a caller-supplied identity value.
                    // When no explicit value is provided, omit the column so SQLite generates
                    // the next rowid automatically and we retrieve it via last_insert_rowid().
                    bool hasExplicitPkValue = IsColumnModified(packet, col);

                    if (hasExplicitPkValue)
                    {
                        // Insert with caller-supplied PK — no auto-increment retrieval needed
                        SQLiteParameter p = types[col.Name];
                        SQLiteParameter clone = CloneParameter(p, col);
                        cmd.Parameters.Add(clone);  

                        into   += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                        values += comma + p.ParameterName;
                        comma   = ", ";
                        // Do NOT set props["AutoInc"] — skip last_insert_rowid() in OnRowUpdated
                    }
                    else
                    {
                        // No explicit PK value — let SQLite generate the rowid
                        props["AutoInc"] = col.Name;
                        props["Source"]  = request.ProviderMetadata.Source;
                    }
                }
                else if (col.IsConcurrency)
                {
                    props["Timestamp"] = col.Name;
                    props["Source"] = request.ProviderMetadata.Source;
                }
                else if (col.IsEntitySpacesConcurrency)
                {
                    props["EntitySpacesConcurrency"] = col.Name;

                    SQLiteParameter p = types[col.Name];

                    into += comma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    values += comma + "1";
                    comma = ", ";

                    SQLiteParameter clone = CloneParameter(p);
                    clone.Value = 1;
                    cmd.Parameters.Add(clone);
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
                    defaults += defaultComma + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose;
                    defaultComma = ",";
                }

                if (col.IsInPrimaryKey)
                {
                    where += whereComma + col.Name;
                    whereComma = ",";
                }
            }

            #region Special Columns
            // [FIX-PHASE-4] Server-side special columns are excluded from the INSERT
            // column list. The ProviderMetadata["X.ServerSideText"] value is the
            // bareword column name (e.g. "DateAdded"), which SQLite resolves in a
            // VALUES context as a column reference and rejects with
            // "no such column: DateAdded".
            //
            // The DB DEFAULT fills these columns on INSERT. Their generated values
            // are fetched back through the Defaults mechanism in OnRowUpdated,
            // which runs a SELECT against the just-inserted row. This mirrors the
            // pattern already applied in the PostgreSQL and SQL Server providers.
            if (cols.DateAdded != null && cols.DateAdded.IsServerSide)
            {
                defaults += defaultComma + Delimiters.ColumnOpen + cols.DateAdded.ColumnName + Delimiters.ColumnClose;
                defaultComma = ",";
            }

            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                defaults += defaultComma + Delimiters.ColumnOpen + cols.DateModified.ColumnName + Delimiters.ColumnClose;
                defaultComma = ",";
            }

            if (cols.AddedBy != null && cols.AddedBy.IsServerSide)
            {
                defaults += defaultComma + Delimiters.ColumnOpen + cols.AddedBy.ColumnName + Delimiters.ColumnClose;
                defaultComma = ",";
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                defaults += defaultComma + Delimiters.ColumnOpen + cols.ModifiedBy.ColumnName + Delimiters.ColumnClose;
                defaultComma = ",";
            }
            #endregion

            if (defaults.Length > 0)
            {
                comma = String.Empty;
                props["Defaults"] = defaults;
                props["Where"] = where;
            }

            sql += " INSERT INTO " + CreateFullName(request);

            if (into.Length != 0)
            {
                sql += "(" + into + ") VALUES (" + values + ")";
            }
            else
            {
                sql += "DEFAULT VALUES";
            }

            request.Properties = props;

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        static public SQLiteCommand BuildDynamicUpdateCommand(esDataRequest request, esEntitySavePacket packet)
        {
            string where = String.Empty;
            string scomma = String.Empty;
            string wcomma = String.Empty;
            string defaults = String.Empty;
            string defaultsWhere = String.Empty;
            string defaultsComma = String.Empty;
            string defaultsWhereComma = String.Empty;

            string sql = "UPDATE " + CreateFullName(request) + " SET ";

            PropertyCollection props = new PropertyCollection();

            Dictionary<string, SQLiteParameter> types = Cache.GetParameters(request);

            SQLiteCommand cmd = new SQLiteCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            esColumnMetadataCollection cols = request.Columns;
            foreach (esColumnMetadata col in cols)
            {
                bool isModified = IsColumnModified(packet, col);

                if (isModified && (!col.IsAutoIncrement && !col.IsConcurrency && !col.IsEntitySpacesConcurrency))
                {
                    SQLiteParameter p = types[col.Name];
                    cmd.Parameters.Add(CloneParameter(p, col));

                    sql += scomma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    scomma = ", ";
                }
                else if (col.IsAutoIncrement)
                {
                    // Nothing to do but leave this here
                }
                else if (col.IsConcurrency)
                {
                    SQLiteParameter p = types[col.Name];
                    p = CloneParameter(p, col);
                    p.SourceVersion = DataRowVersion.Original;
                    cmd.Parameters.Add(p);

                    where += wcomma;
                    where += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    wcomma = " AND ";
                }
                else if (col.IsEntitySpacesConcurrency)
                {
                    props["EntitySpacesConcurrency"] = col.Name;

                    SQLiteParameter p = types[col.Name];
                    p = CloneParameter(p, col);
                    p.SourceVersion = DataRowVersion.Original;
                    cmd.Parameters.Add(p);

                    sql += scomma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " + 1";
                    scomma = ", ";

                    where += wcomma;
                    where += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    wcomma = " AND ";
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
                    SQLiteParameter p = types[col.Name];
                    p = CloneParameter(p, col);
                    p.SourceVersion = DataRowVersion.Original;
                    cmd.Parameters.Add(p);

                    where += wcomma;
                    where += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    wcomma = " AND ";

                    defaultsWhere += defaultsWhereComma + col.Name;
                    defaultsWhereComma = ",";
                }
            }

            #region Special Columns
            // [FIX-PHASE-4] Server-side special columns are not emitted in the SET
            // clause. The ProviderMetadata["X.ServerSideText"] value is the bareword
            // column name, producing "[DateModified] = DateModified", which is
            // either a no-op (SQLite without a trigger) or a syntax error
            // (PostgreSQL without a quoted identifier).
            //
            // DateAdded and AddedBy are never touched by an UPDATE. DateModified
            // and ModifiedBy are read back through the Defaults mechanism after
            // the UPDATE completes. This mirrors the pattern already applied in
            // the PostgreSQL and SQL Server providers.
            if (cols.DateModified != null && cols.DateModified.IsServerSide)
            {
                defaults += defaultsComma + Delimiters.ColumnOpen + cols.DateModified.ColumnName + Delimiters.ColumnClose;
                defaultsComma = ",";
            }

            if (cols.ModifiedBy != null && cols.ModifiedBy.IsServerSide)
            {
                defaults += defaultsComma + Delimiters.ColumnOpen + cols.ModifiedBy.ColumnName + Delimiters.ColumnClose;
                defaultsComma = ",";
            }
            #endregion

            if (defaults.Length > 0)
            {
                props["Defaults"] = defaults;
                props["Where"] = defaultsWhere;
            }

            sql += " WHERE (" + where + ")";

            request.Properties = props;

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        static public SQLiteCommand BuildDynamicDeleteCommand(esDataRequest request)
        {
            Dictionary<string, SQLiteParameter> types = Cache.GetParameters(request);

            SQLiteCommand cmd = new SQLiteCommand();
            if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

            string sql = "DELETE FROM " + CreateFullName(request) + " ";

            string comma = String.Empty;
            comma = String.Empty;
            sql += " WHERE ";
            foreach (esColumnMetadata col in request.Columns)
            {
                if (col.IsInPrimaryKey || col.IsEntitySpacesConcurrency)
                {
                    SQLiteParameter p = types[col.Name];
                    cmd.Parameters.Add(CloneParameter(p, col));

                    sql += comma;
                    sql += Delimiters.ColumnOpen + col.Name + Delimiters.ColumnClose + " = " + p.ParameterName;
                    comma = " AND ";
                }
            }

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        static public SQLiteCommand BuildStoredProcInsertCommand(esDataRequest request)
        {
            return null;
        }

        static public SQLiteCommand BuildStoredProcUpdateCommand(esDataRequest request)
        {
            return null;
        }

        static public SQLiteCommand BuildStoredProcDeleteCommand(esDataRequest request)
        {
            return null;
        }

        static public void PopulateStoredProcParameters(SQLiteCommand cmd, esDataRequest request)
        {
            Dictionary<string, SQLiteParameter> types = Cache.GetParameters(request);

            SQLiteParameter p;

            foreach (esColumnMetadata col in request.Columns)
            {
                p = types[col.Name];
                p = CloneParameter(p, col);
                p.SourceVersion = DataRowVersion.Current;

                if (col.IsComputed && col.CharacterMaxLength > 0)
                {
                    p.Size = (int)col.CharacterMaxLength;
                }

                cmd.Parameters.Add(p);
            }
        }

        static private SQLiteParameter CloneParameter(SQLiteParameter p, esColumnMetadata col = null)
        {
            ICloneable param = p as ICloneable;
            SQLiteParameter clone = param.Clone() as SQLiteParameter;

            // [DECIMAL PRECISION FIX]
            // SQLite has no native DECIMAL type. When the driver binds a parameter
            // declared as DbType.Decimal, it serializes the value via
            // CultureInfo.CurrentCulture. On comma-decimal cultures (es-CL, es-ES,
            // de-DE, ...), 123.45m becomes "123,45". SQLite's NUMERIC affinity
            // parser stops at the comma and stores 123 (INTEGER), silently
            // truncating the fractional part.
            //
            // Forcing DbType.Double makes the driver use sqlite3_bind_double, which
            // stores the value as REAL. REAL round-trips back to decimal without
            // precision loss for the DECIMAL(10,2) and DECIMAL(10,5) ranges used in
            // the Billing schema.
            //
            // Two detection paths:
            //   1. DbType check: catches cases where the Cache mapped
            //      esSystemType.Decimal to DbType.Decimal or DbType.Currency.
            //   2. Metadata check: catches cases where the Cache mapped it to
            //      something else (String, VarNumeric). Passing col explicitly
            //      lets us look at the authoritative source — the entity metadata.
            //
            // Observed on BillingInvoice.Total (DECIMAL(10,2)) with es-CL:
            // before this metadata-aware branch, the DbType check never fired
            // because Cache returned DbType.String. 123.45m was stored as 123.
            if (clone.DbType == DbType.Decimal
                || clone.DbType == DbType.Currency
                || clone.DbType == DbType.VarNumeric
                || (col != null && col.esType == esSystemType.Decimal))
            {
                clone.DbType = DbType.Double;
            }

            return clone;
        }

        static public string CreateFullName(esDynamicQuery query)
        {
            IDynamicQueryInternal iQuery = query as IDynamicQueryInternal;
            esProviderSpecificMetadata providerMetadata = iQuery.ProviderMetadata as esProviderSpecificMetadata;

            string name = String.Empty;

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

            name += Delimiters.TableOpen;
            if(request.DynamicQuery != null && request.DynamicQuery.querySource != null)
                name += request.DynamicQuery.querySource;
            else
                name += request.QueryText != null ? request.QueryText : request.ProviderMetadata.Destination;
            name += Delimiters.TableClose;

            return name;
        }

        static public string CreateFullName(esProviderSpecificMetadata providerMetadata)
        {
            return Delimiters.TableOpen + providerMetadata.Destination + Delimiters.TableClose;
        }

        static public esConcurrencyException CheckForConcurrencyException(SQLiteException ex)
        {
            // SQLite error codes relevant to concurrency / constraint violations:
            //   19  SQLITE_CONSTRAINT  — general constraint failure (UNIQUE, PK, FK, CHECK, NOT NULL)
            //   11  SQLITE_CORRUPT     — database file corrupt (not a concurrency issue, skip)
            // System.Data.SQLite exposes the numeric code via SQLiteException.ErrorCode.
            // We map SQLITE_CONSTRAINT (19) to esConcurrencyException so EntitySpaces callers
            // receive the same typed exception they get from other providers (PostgreSQL 23505, etc.).
            if (ex != null && (int)ex.ErrorCode == 19)
            {
                return new esConcurrencyException(ex.Message, ex);
            }

            return null;
        }

        static public void AddParameters(SQLiteCommand cmd, esDataRequest request)
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
                SQLiteParameter param;

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
                            break;

                        case esParameterDirection.ReturnValue:
                            param.Direction = ParameterDirection.ReturnValue;
                            break;

                        // The default is ParameterDirection.Input;
                    }
                }
            }
        }

        static public void GatherReturnParameters(SQLiteCommand cmd, esDataRequest request, esDataResponse response)
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
                            SQLiteParameter p = cmd.Parameters[Delimiters.Param + esParam.Name];
                            esParam.Value = p.Value;
                        }
                    }
                }
            }
        }

        // ======================================================================
        // [NEW] Tolerant column/property name resolution.
        //
        // The Studio-generated ApplyPostSaveKeys calls SetProperty with the
        // property name ("Invoiceid"), which writes into ModifiedColumns and
        // CurrentValues under that key. The provider's SQL builders and the
        // adapter-based SetModifiedValues / SetOriginalValues helpers must
        // recognize both the DB column name ("invoiceid") and the generated
        // property name when they diverge.
        //
        // On models where column name and property name coincide (the rest of
        // the Northwind schema, all SQL Server cases), these helpers are no-ops.
        // ======================================================================

        /// <summary>
        /// True if the column is present in ModifiedColumns under either the
        /// DB column name or the EntitySpaces property name.
        /// </summary>
        static private bool IsColumnModified(esEntitySavePacket packet, esColumnMetadata col)
        {
            if (packet.ModifiedColumns == null) return false;
            if (packet.ModifiedColumns.Contains(col.Name)) return true;
            if (!string.IsNullOrEmpty(col.PropertyName) &&
                packet.ModifiedColumns.Contains(col.PropertyName)) return true;
            return false;
        }

        /// <summary>
        /// Reads a column value from CurrentValues, accepting either the DB
        /// column name or the property name. Checks the value slot (not just
        /// ContainsKey) because esSmartDictionary registers all column names
        /// and returns null for unassigned ones.
        /// </summary>
        static private object GetCurrentValue(esEntitySavePacket packet, esColumnMetadata col)
        {
            object byColumn = null;
            if (packet.CurrentValues.ContainsKey(col.Name))
                byColumn = packet.CurrentValues[col.Name];

            if (byColumn != null && byColumn != DBNull.Value)
                return byColumn;

            if (!string.IsNullOrEmpty(col.PropertyName) &&
                packet.CurrentValues.ContainsKey(col.PropertyName))
            {
                object byProperty = packet.CurrentValues[col.PropertyName];
                if (byProperty != null && byProperty != DBNull.Value)
                    return byProperty;
            }

            return byColumn;
        }

        /// <summary>
        /// Same as GetCurrentValue, applied to OriginalValues.
        /// </summary>
        static private object GetOriginalValue(esEntitySavePacket packet, esColumnMetadata col)
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

        /// <summary>
        /// Finds an esColumnMetadata by either the DB column name or the
        /// property name. Used to resolve entries in ModifiedColumns that were
        /// keyed by property name.
        /// </summary>
        static public esColumnMetadata FindColumnByEitherName(esColumnMetadataCollection columns, string name)
        {
            if (columns == null || string.IsNullOrEmpty(name)) return null;

            var byColumn = columns.FindByColumnName(name);
            if (byColumn != null) return byColumn;

            foreach (esColumnMetadata c in columns)
            {
                if (!string.IsNullOrEmpty(c.PropertyName) &&
                    string.Equals(c.PropertyName, name, StringComparison.Ordinal))
                    return c;
            }

            return null;
        }

    } // end class
}

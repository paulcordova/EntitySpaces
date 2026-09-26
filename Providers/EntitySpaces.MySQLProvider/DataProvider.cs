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
using System.Data;
using System.Collections.Generic;

using EntitySpaces.DynamicQuery;
using EntitySpaces.Interfaces;

using MySqlConnector;
using System.Threading;
using System.Diagnostics;

namespace EntitySpaces.MySQLProvider
{
    public class DataProvider : IDataProvider
    {
        public DataProvider()
        {

        }

        #region esTraceArguments

        private sealed class esTraceArguments : EntitySpaces.Interfaces.ITraceArguments, IDisposable
        {
            static private long packetOrder = 0;

            private sealed class esTraceParameter : ITraceParameter
            {
                public string Name { get; set; }
                public string Direction { get; set; }
                public string ParamType { get; set; }
                public string BeforeValue { get; set; }
                public string AfterValue { get; set; }
            }

            public esTraceArguments()
            {

            }

            public esTraceArguments(esDataRequest request, IDbCommand cmd, esEntitySavePacket packet, string action, string callStack)
            {
                PacketOrder = Interlocked.Increment(ref esTraceArguments.packetOrder);

                this.command = cmd;

                TraceChannel = DataProvider.sTraceChannel;
                Syntax = "MYSQL";
                Request = request;
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                Action = action;
                CallStack = callStack;
                SqlCommand = cmd;
                ApplicationName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                IDataParameterCollection parameters = cmd.Parameters;

                if (parameters.Count > 0)
                {
                    Parameters = new List<ITraceParameter>(parameters.Count);

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        MySqlParameter param = parameters[i] as MySqlParameter;

                        esTraceParameter p = new esTraceParameter()
                        {
                            Name = param.ParameterName,
                            Direction = param.Direction.ToString(),
                            ParamType = param.MySqlDbType.ToString().ToUpper(),
                            BeforeValue = param.Value != null && param.Value != DBNull.Value ? Convert.ToString(param.Value) : "null"
                        };

                        try
                        {
                            // Let's make it look like we're using parameters for the profiler
                            if (param.Value == null || param.Value == DBNull.Value)
                            {
                                if (param.SourceVersion == DataRowVersion.Current || param.SourceVersion == DataRowVersion.Default)
                                {
                                    object o = packet.CurrentValues[param.SourceColumn];
                                    if (o != null && o != DBNull.Value)
                                    {
                                        p.BeforeValue = Convert.ToString(o);
                                    }
                                }
                                else if (param.SourceVersion == DataRowVersion.Original)
                                {
                                    object o = packet.OriginalValues[param.SourceColumn];
                                    if (o != null && o != DBNull.Value)
                                    {
                                        p.BeforeValue = Convert.ToString(o);
                                    }
                                }
                            }
                        }
                        catch { }

                        this.Parameters.Add(p);
                    }
                }

                stopwatch = Stopwatch.StartNew();
            }

            public esTraceArguments(esDataRequest request, IDbCommand cmd, string action, string callStack)
            {
                PacketOrder = Interlocked.Increment(ref esTraceArguments.packetOrder);

                this.command = cmd;

                TraceChannel = DataProvider.sTraceChannel;
                Syntax = "MYSQL";
                Request = request;
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                Action = action;
                CallStack = callStack;
                SqlCommand = cmd;
                ApplicationName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                IDataParameterCollection parameters = cmd.Parameters;

                if (parameters.Count > 0)
                {
                    Parameters = new List<ITraceParameter>(parameters.Count);

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        MySqlParameter param = parameters[i] as MySqlParameter;

                        esTraceParameter p = new esTraceParameter()
                        {
                            Name = param.ParameterName,
                            Direction = param.Direction.ToString(),
                            ParamType = param.MySqlDbType.ToString().ToUpper(),
                            BeforeValue = param.Value != null && param.Value != DBNull.Value ? Convert.ToString(param.Value) : "null"
                        };

                        this.Parameters.Add(p);
                    }
                }

                stopwatch = Stopwatch.StartNew();
            }

            // Temporary variable
            private IDbCommand command;

            public long PacketOrder { get; set; }
            public string Syntax { get; set; }
            public esDataRequest Request { get; set; }
            public int ThreadId { get; set; }
            public string Action { get; set; }
            public string CallStack { get; set; }
            public IDbCommand SqlCommand { get; set; }
            public string ApplicationName { get; set; }
            public string TraceChannel { get; set; }
            public long Duration { get; set; }
            public long Ticks { get; set; }
            public string Exception { get; set; }
            public List<ITraceParameter> Parameters { get; set; }

            private Stopwatch stopwatch;

            void IDisposable.Dispose()
            {
                stopwatch.Stop();
                Duration = stopwatch.ElapsedMilliseconds;
                Ticks = stopwatch.ElapsedTicks;

                // Gather Output Parameters
                if (this.Parameters != null && this.Parameters.Count > 0)
                {
                    IDataParameterCollection parameters = command.Parameters;

                    for (int i = 0; i < this.Parameters.Count; i++)
                    {
                        ITraceParameter esParam = this.Parameters[i];
                        IDbDataParameter param = parameters[esParam.Name] as IDbDataParameter;

                        if (param.Direction == ParameterDirection.InputOutput || param.Direction == ParameterDirection.Output)
                        {
                            esParam.AfterValue = param.Value != null ? Convert.ToString(param.Value) : "null";
                        }
                    }
                }

                DataProvider.sTraceHandler(this);
            }
        }

        #endregion

        #region Profiling Logic

        /// <summary>
        /// The EventHandler used to decouple the profiling code from the core assemblies
        /// </summary>
        event TraceEventHandler IDataProvider.TraceHandler
        {
            add { DataProvider.sTraceHandler += value; }
            remove { DataProvider.sTraceHandler -= value; }
        }
        static private event TraceEventHandler sTraceHandler;

        /// <summary>
        /// Returns true if this Provider is current being profiled
        /// </summary>
        bool IDataProvider.IsTracing
        {
            get
            {
                return sTraceHandler != null ? true : false;
            }
        }

        /// <summary>
        /// Used to set the Channel this provider is to use during Profiling
        /// </summary>
        string IDataProvider.TraceChannel
        {
            get { return DataProvider.sTraceChannel; }
            set { DataProvider.sTraceChannel = value; }
        }
        static private string sTraceChannel = "Channel1";

        #endregion

        /// <summary>
        /// This method acts as a delegate for esTransactionScope
        /// </summary>
        /// <returns></returns>
        static private IDbConnection CreateIDbConnectionDelegate()
        {
            return new MySqlConnection();
        }

        static private void CleanupCommand(MySqlCommand cmd)
        {
            if (cmd != null && cmd.Connection != null)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }
            }
        }

        #region IDataProvider Members

        esDataResponse IDataProvider.esLoadDataTable(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();

            try
            {
                switch (request.QueryType)
                {
                    case esQueryType.StoredProcedure:

                        response = LoadDataTableFromStoredProcedure(request);
                        break;

                    case esQueryType.Text:

                        response = LoadDataTableFromText(request);
                        break;

                    case esQueryType.DynamicQuery:

                        response = new esDataResponse();
                        MySqlCommand cmd = QueryBuilder.PrepareCommand(request);
                        LoadDataTableFromDynamicQuery(request, response, cmd);
                        break;

                    case esQueryType.DynamicQueryParseOnly:

                        response = new esDataResponse();
                        MySqlCommand cmd1 = QueryBuilder.PrepareCommand(request);
                        response.LastQuery = cmd1.CommandText;
                        break;

                    //case esQueryType.IQueryable:

                    //    response = new esDataResponse();
                    //    LoadDataTableForLinqToSql(request, response);
                    //    break;

                    case esQueryType.ManyToMany:

                        response = LoadManyToMany(request);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        esDataResponse IDataProvider.esSaveDataTable(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();

            try
            {
                if (request.SqlAccessType == esSqlAccessType.StoredProcedure)
                {
                    if (request.CollectionSavePacket != null)
                        SaveStoredProcCollection(request);
                    else
                        SaveStoredProcEntity(request);
                }
                else
                {
                    if (request.EntitySavePacket.CurrentValues == null)
                        SaveDynamicCollection(request);
                    else
                        SaveDynamicEntity(request);
                }
            }
            catch (MySqlException ex)
            {
                esException es = Shared.CheckForConcurrencyException(ex);
                if (es != null)
                    response.Exception = es;
                else
                    response.Exception = ex;
            }
            catch (DBConcurrencyException dbex)
            {
                response.Exception = new esConcurrencyException("Error in MySqlClientProvider.esSaveDataTable", dbex);
            }

            response.Table = request.Table;
            return response;
        }

        esDataResponse IDataProvider.ExecuteNonQuery(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                cmd = new MySqlCommand();
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                switch (request.QueryType)
                {
                    case esQueryType.TableDirect:
                        cmd.CommandType = CommandType.TableDirect;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.StoredProcedure:
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.Text:
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = request.QueryText;
                        break;
                }

                try
                {
                    esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "ExecuteNonQuery", System.Environment.StackTrace))
                        {
                            try
                            {
                                response.RowsEffected = cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        response.RowsEffected = cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — the connection may be enlisted in an
                    // ambient esTransactionScope; issuing ROLLBACK would abort the whole scope,
                    // not just this command. esTransactionScope.Dispose() handles rollback
                    // when the scope ends without Complete().
                    esTransactionScope.DeEnlist(cmd);
                }

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                response.Exception = ex;
            }

            return response;
        }

        esDataResponse IDataProvider.ExecuteReader(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;
            bool needsCleanup = false;

            try
            {
                cmd = new MySqlCommand();
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                switch (request.QueryType)
                {
                    case esQueryType.TableDirect:
                        cmd.CommandType = CommandType.TableDirect;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.StoredProcedure:
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.Text:
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.DynamicQuery:
                        cmd = QueryBuilder.PrepareCommand(request);
                        break;
                }

                cmd.Connection = new MySqlConnection(request.ConnectionString);
                cmd.Connection.Open();

                #region Profiling
                if (sTraceHandler != null)
                {
                    using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "ExecuteReader", System.Environment.StackTrace))
                    {
                        try
                        {
                            response.DataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        }
                        catch (Exception ex)
                        {
                            esTrace.Exception = ex.Message;
                            throw;
                        }
                    }
                }
                else
                #endregion
                {
                    response.DataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                needsCleanup = true;
                throw;
            }
            finally
            {
                // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                // ExecuteReader opens its OWN raw connection (no Enlist), so on error we
                // must close it explicitly: CommandBehavior.CloseConnection only fires
                // when ExecuteReader succeeds and the reader is disposed.
                if (needsCleanup)
                {
                    CleanupCommand(cmd);
                }
            }

            return response;
        }
        esDataResponse IDataProvider.ExecuteScalar(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                cmd = new MySqlCommand();
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                switch (request.QueryType)
                {
                    case esQueryType.TableDirect:
                        cmd.CommandType = CommandType.TableDirect;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.StoredProcedure:
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.Text:
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.DynamicQuery:
                        cmd = QueryBuilder.PrepareCommand(request);
                        break;
                }

                try
                {
                    esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "ExecuteScalar", System.Environment.StackTrace))
                        {
                            try
                            {
                                response.Scalar = cmd.ExecuteScalar();
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        response.Scalar = cmd.ExecuteScalar();
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(cmd);
                }

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                response.Exception = ex;
            }

            return response;
        }

        esDataResponse IDataProvider.FillDataSet(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();

            try
            {
                switch (request.QueryType)
                {
                    case esQueryType.StoredProcedure:

                        response = LoadDataSetFromStoredProcedure(request);
                        break;

                    case esQueryType.Text:

                        response = LoadDataSetFromText(request);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        esDataResponse IDataProvider.FillDataTable(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();

            try
            {
                switch (request.QueryType)
                {
                    case esQueryType.StoredProcedure:

                        response = LoadDataTableFromStoredProcedure(request);
                        break;

                    case esQueryType.Text:

                        response = LoadDataTableFromText(request);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        #endregion

        static private esDataResponse LoadDataSetFromStoredProcedure(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                DataSet dataSet = new DataSet();

                cmd = new MySqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = request.QueryText;

                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadFromStoredProcedure", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataSet);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataSet);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.DataSet = dataSet;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return response;
        }

        static private esDataResponse LoadDataSetFromText(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;
            bool hasError = false;

            try
            {
                DataSet dataSet = new DataSet();

                cmd = new MySqlCommand();
                cmd.CommandType = CommandType.Text;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                MySqlDataAdapter da = new MySqlDataAdapter();
                cmd.CommandText = request.QueryText;
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadDataSetFromText", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataSet);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                hasError = true;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataSet);
                    }
                }
                finally
                {
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.DataSet = dataSet;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception)
            {
                hasError = true;
                CleanupCommand(cmd);
                throw;
            }
            finally
            {
                // [REVISED] No explicit ROLLBACK — the connection may be enlisted in an
                // ambient esTransactionScope; issuing ROLLBACK would abort the whole scope,
                // not just this command. esTransactionScope.Dispose() handles rollback
                // when the scope ends without Complete().
                esTransactionScope.DeEnlist(cmd);
            }

            return response;
        }

        static private esDataResponse LoadDataTableFromStoredProcedure(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new MySqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = request.QueryText;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadFromStoredProcedure", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataTable);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataTable);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return response;
        }

        static private esDataResponse LoadDataTableFromText(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new MySqlCommand();
                cmd.CommandType = CommandType.Text;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                MySqlDataAdapter da = new MySqlDataAdapter();
                cmd.CommandText = request.QueryText;
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadFromText", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataTable);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataTable);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return response;
        }

        static private esDataResponse LoadManyToMany(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            MySqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new MySqlCommand();
                cmd.CommandType = CommandType.Text;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

                string mmQuery = request.QueryText;

                string[] sections = mmQuery.Split('|');
                string[] tables = sections[0].Split(',');
                string[] columns = sections[1].Split(',');

                // We build the query, we don't use Delimiters to avoid tons of extra concatenation
                string sql = "SELECT * FROM `" + tables[0];
                sql += "` JOIN `" + tables[1] + "` ON `" + tables[0] + "`.`" + columns[0] + "` = `";
                sql += tables[1] + "`.`" + columns[1];
                sql += "` WHERE `" + tables[1] + "`.`" + sections[2] + "` = ?";

                if (request.Parameters != null)
                {
                    foreach (esParameter esParam in request.Parameters)
                    {
                        sql += esParam.Name;
                    }

                    Shared.AddParameters(cmd, request);
                }

                MySqlDataAdapter da = new MySqlDataAdapter();
                cmd.CommandText = sql;
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadManyToMany", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataTable);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataTable);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }

            return response;
        }

        // This is used only to execute the Dynamic Query API
        static private void LoadDataTableFromDynamicQuery(esDataRequest request, esDataResponse response, MySqlCommand cmd)
        {
            try
            {
                response.LastQuery = cmd.CommandText;

                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                MySqlDataAdapter da = new MySqlDataAdapter();
                da.SelectCommand = cmd;

                try
                {
                    esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadFromDynamicQuery", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Fill(dataTable);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Fill(dataTable);
                    }
                }
                finally
                {
                    // [REVISED] No explicit Rollback on cmd.Transaction — that would abort
                    // the ambient esTransactionScope. See ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;
            }
            catch (Exception)
            {
                CleanupCommand(cmd);
                throw;
            }
        }

        #region LINQ

        // This is used only to execute the Dynamic Query API
        //static private void LoadDataTableForLinqToSql(esDataRequest request, esDataResponse response)
        //{
        //    MySqlCommand cmd = null;

        //    try
        //    {
        //        DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

        //        cmd = request.LinqContext.GetCommand(request.LinqQuery) as MySqlCommand;

        //        response.LastQuery = cmd.CommandText;

        //        if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

        //        MySqlDataAdapter da = new MySqlDataAdapter();
        //        da.SelectCommand = cmd;

        //        try
        //        {
        //            esTransactionScope.Enlist(da.SelectCommand, request.ConnectionString, CreateIDbConnectionDelegate);

        //            #region Profiling
        //            if (sTraceHandler != null)
        //            {
        //                using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "LoadForLinqToSql", System.Environment.StackTrace))
        //                {
        //                    try
        //                    {
        //                        da.Fill(dataTable);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        esTrace.Exception = ex.Message;
        //                        throw;
        //                    }
        //                }
        //            }
        //            else
        //            #endregion
        //            {
        //                da.Fill(dataTable);
        //            }
        //        }
        //        finally
        //        {
        //            esTransactionScope.DeEnlist(da.SelectCommand);
        //        }

        //        response.Table = dataTable;

        //        if (request.Parameters != null)
        //        {
        //            Shared.GatherReturnParameters(cmd, request, response);
        //        }
        //    }
        //    catch
        //    {
        //        CleanupCommand(cmd);
        //        throw;
        //    }
        //    finally
        //    {

        //    }
        //}

        #endregion

        static private DataTable SaveStoredProcCollection(esDataRequest request)
        {
            bool needToInsert = false;
            bool needToUpdate = false;
            bool needToDelete = false;

            Dictionary<DataRow, esEntitySavePacket> rowMapping = null;

            if (request.ContinueUpdateOnError)
            {
                rowMapping = new Dictionary<DataRow, esEntitySavePacket>();
            }

            //================================================
            // Create the DataTable ...
            //================================================
            DataTable dataTable = CreateDataTable(request);

            foreach (esEntitySavePacket packet in request.CollectionSavePacket)
            {
                DataRow row = dataTable.NewRow();

                switch (request.EntitySavePacket.RowState)
                {
                    case esDataRowState.Added:
                        SetModifiedValues(request, packet, row);
                        dataTable.Rows.Add(row);
                        if (request.ContinueUpdateOnError) rowMapping[row] = packet;
                        break;

                    case esDataRowState.Modified:
                        SetOriginalValues(request, packet, row, false);
                        SetModifiedValues(request, packet, row);
                        dataTable.Rows.Add(row);
                        row.AcceptChanges();
                        row.SetModified();
                        if (request.ContinueUpdateOnError) rowMapping[row] = packet;
                        break;

                    case esDataRowState.Deleted:
                        SetOriginalValues(request, packet, row, true);
                        dataTable.Rows.Add(row);
                        row.AcceptChanges();
                        row.Delete();
                        if (request.ContinueUpdateOnError) rowMapping[row] = packet;
                        break;
                }
            }

            if (Shared.HasUpdates(dataTable.Rows, out needToInsert, out needToUpdate, out needToDelete))
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter())
                {
                    da.AcceptChangesDuringUpdate = false;

                    MySqlCommand cmd = null;

                    if (needToInsert) da.InsertCommand = cmd = Shared.BuildStoredProcInsertCommand(request);
                    if (needToUpdate) da.UpdateCommand = cmd = Shared.BuildStoredProcUpdateCommand(request);
                    if (needToDelete) da.DeleteCommand = cmd = Shared.BuildStoredProcDeleteCommand(request);

                    using (esTransactionScope scope = new esTransactionScope())
                    {
                        if (needToInsert) esTransactionScope.Enlist(da.InsertCommand, request.ConnectionString, CreateIDbConnectionDelegate);
                        if (needToUpdate) esTransactionScope.Enlist(da.UpdateCommand, request.ConnectionString, CreateIDbConnectionDelegate);
                        if (needToDelete) esTransactionScope.Enlist(da.DeleteCommand, request.ConnectionString, CreateIDbConnectionDelegate);

                        try
                        {
                            #region Profiling
                            if (sTraceHandler != null)
                            {
                                using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveCollectionStoredProcedure", System.Environment.StackTrace))
                                {
                                    try
                                    {
                                        da.Update(dataTable);
                                    }
                                    catch (Exception ex)
                                    {
                                        esTrace.Exception = ex.Message;
                                        throw;
                                    }
                                }
                            }
                            else
                            #endregion
                            {
                                da.Update(dataTable);
                            }

                            // [C2 FIX — complement] Sync keys for each saved packet.
                            foreach (esEntitySavePacket pkt in request.CollectionSavePacket)
                            {
                                if (pkt.CurrentValues != null)
                                    SyncColumnAndPropertyKeys(pkt, request.Columns);
                            }
                        }
                        finally
                        {
                            if (needToInsert) esTransactionScope.DeEnlist(da.InsertCommand);
                            if (needToUpdate) esTransactionScope.DeEnlist(da.UpdateCommand);
                            if (needToDelete) esTransactionScope.DeEnlist(da.DeleteCommand);
                        }

                        scope.Complete();
                    }
                }

                if (request.ContinueUpdateOnError && dataTable.HasErrors)
                {
                    DataRow[] errors = dataTable.GetErrors();

                    foreach (DataRow rowWithError in errors)
                    {
                        request.FireOnError(rowMapping[rowWithError], rowWithError.RowError);
                    }
                }
            }

            return request.Table;
        }

        static private DataTable SaveStoredProcEntity(esDataRequest request)
        {
            DataTable dataTable = CreateDataTable(request);

            using (MySqlDataAdapter da = new MySqlDataAdapter())
            {
                da.AcceptChangesDuringUpdate = false;

                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);

                MySqlCommand cmd = null;

                switch (request.EntitySavePacket.RowState)
                {
                    case esDataRowState.Added:
                        cmd = da.InsertCommand = Shared.BuildStoredProcInsertCommand(request);
                        SetModifiedValues(request, request.EntitySavePacket, row);
                        break;

                    case esDataRowState.Modified:
                        cmd = da.UpdateCommand = Shared.BuildStoredProcUpdateCommand(request);
                        SetOriginalValues(request, request.EntitySavePacket, row, false);
                        SetModifiedValues(request, request.EntitySavePacket, row);
                        row.AcceptChanges();
                        row.SetModified();
                        break;

                    case esDataRowState.Deleted:
                        cmd = da.DeleteCommand = Shared.BuildStoredProcDeleteCommand(request);
                        SetOriginalValues(request, request.EntitySavePacket, row, true);
                        row.AcceptChanges();
                        row.Delete();
                        break;
                }

                DataRow[] singleRow = new DataRow[1];
                singleRow[0] = row;

                esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                try
                {
                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveEntityStoredProcedure", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Update(singleRow);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Update(singleRow);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(cmd);
                }

                // [C2 FIX — reordered] Copy Output/InputOutput params FIRST, then sync.
                // Previous order ran sync before the output copy, so a stored proc that
                // returned AutoInc (or any other server-generated value) as an output
                // parameter left the value only under the column name — the property
                // name slot stayed empty and the entity getter returned null.
                if (request.EntitySavePacket.RowState != esDataRowState.Deleted && cmd.Parameters != null)
                {
                    foreach (MySqlParameter param in cmd.Parameters)
                    {
                        switch (param.Direction)
                        {
                            case ParameterDirection.Output:
                            case ParameterDirection.InputOutput:
                                request.EntitySavePacket.CurrentValues[param.SourceColumn] = param.Value;
                                break;
                        }
                    }
                }

                // Now that both Output params and entity values are in CurrentValues,
                // synchronize column-name and property-name slots.
                SyncColumnAndPropertyKeys(request.EntitySavePacket, request.Columns);
            }

            return dataTable;
        }

        static private DataTable SaveDynamicCollection(esDataRequest request)
        {
            esEntitySavePacket pkt = request.CollectionSavePacket[0];

            if (pkt.RowState == esDataRowState.Deleted)
            {
                //============================================================================
                // We do all our deletes at once, so if the first one is a delete they all are
                //============================================================================
                return SaveDynamicCollection_Deletes(request);
            }
            else
            {
                //============================================================================
                // We do all our Inserts and Updates at once
                //============================================================================
                return SaveDynamicCollection_InsertsUpdates(request);
            }
        }

        static private DataTable SaveDynamicCollection_InsertsUpdates(esDataRequest request)
        {
            DataTable dataTable = CreateDataTable(request);

            using (esTransactionScope scope = new esTransactionScope())
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter())
                {
                    da.AcceptChangesDuringUpdate = false;
                    da.ContinueUpdateOnError = request.ContinueUpdateOnError;

                    MySqlCommand cmd = null;

                    // [C5 FIX — optional] Command reuse for homogeneous bulk saves.
                    // Most collections come from a single entity type with the same
                    // modified-columns signature. Rebuilding the command on every row
                    // is safe but wasteful. Cache the signature and only rebuild when
                    // the column set (or row state) actually changes.
                    string lastSignature = null;

                    if (!request.IgnoreComputedColumns)
                    {
                        da.RowUpdated += new MySqlRowUpdatedEventHandler(OnRowUpdated);
                    }

                    foreach (esEntitySavePacket packet in request.CollectionSavePacket)
                    {
                        if (packet.RowState != esDataRowState.Added && packet.RowState != esDataRowState.Modified)
                            continue;

                        DataRow row = dataTable.NewRow();
                        dataTable.Rows.Add(row);

                        // Build a signature that captures row state and modified columns.
                        // Two packets with the same signature produce identical SQL and
                        // parameter layout, so the command object can be reused safely.
                        string signature = packet.RowState + "|" +
                            string.Join(",", packet.ModifiedColumns ?? new List<string>());

                        bool sameSignature = signature == lastSignature;

                        switch (packet.RowState)
                        {
                            case esDataRowState.Added:
                                if (!sameSignature || da.InsertCommand == null)
                                {
                                    cmd = da.InsertCommand = Shared.BuildDynamicInsertCommand(request, packet.ModifiedColumns);
                                    lastSignature = signature;
                                }
                                else
                                {
                                    cmd = da.InsertCommand;
                                }
                                SetModifiedValues(request, packet, row);
                                break;

                            case esDataRowState.Modified:
                                if (!sameSignature || da.UpdateCommand == null)
                                {
                                    cmd = da.UpdateCommand = Shared.BuildDynamicUpdateCommand(request, packet.ModifiedColumns);
                                    lastSignature = signature;
                                }
                                else
                                {
                                    cmd = da.UpdateCommand;
                                }
                                SetOriginalValues(request, packet, row, false);
                                SetModifiedValues(request, packet, row);
                                row.AcceptChanges();
                                row.SetModified();
                                break;
                        }

                        request.Properties["esDataRequest"] = request;
                        request.Properties["esEntityData"] = packet;
                        dataTable.ExtendedProperties["props"] = request.Properties;

                        DataRow[] singleRow = new DataRow[1];
                        singleRow[0] = row;

                        try
                        {
                            esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                            #region Profiling
                            if (sTraceHandler != null)
                            {
                                using (esTraceArguments esTrace = new esTraceArguments(request, cmd, packet, "SaveCollectionDynamic", System.Environment.StackTrace))
                                {
                                    try
                                    {
                                        da.Update(singleRow);
                                    }
                                    catch (Exception ex)
                                    {
                                        esTrace.Exception = ex.Message;
                                        throw;
                                    }
                                }
                            }
                            else
                            #endregion
                            {
                                da.Update(singleRow);
                            }

                            if (row.HasErrors)
                            {
                                request.FireOnError(packet, row.RowError);
                            }
                        }
                        finally
                        {
                            // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                            esTransactionScope.DeEnlist(cmd);
                        }

                        // [C2 FIX — reordered] Copy Output/InputOutput params FIRST,
                        // then sync keys. See SaveStoredProcEntity for rationale.
                        if (!row.HasErrors && packet.RowState != esDataRowState.Deleted && cmd.Parameters != null)
                        {
                            foreach (MySqlParameter param in cmd.Parameters)
                            {
                                switch (param.Direction)
                                {
                                    case ParameterDirection.Output:
                                    case ParameterDirection.InputOutput:
                                        packet.CurrentValues[param.SourceColumn] = param.Value;
                                        break;
                                }
                            }
                        }

                        if (!row.HasErrors)
                        {
                            SyncColumnAndPropertyKeys(packet, request.Columns);
                        }
                    }
                }

                scope.Complete();
            }

            return dataTable;
        }

        static private DataTable SaveDynamicCollection_Deletes(esDataRequest request)
        {
            MySqlCommand cmd = null;

            DataTable dataTable = CreateDataTable(request);

            using (esTransactionScope scope = new esTransactionScope())
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter())
                {
                    da.AcceptChangesDuringUpdate = false;
                    da.ContinueUpdateOnError = request.ContinueUpdateOnError;

                    try
                    {
                        cmd = da.DeleteCommand = Shared.BuildDynamicDeleteCommand(request, request.CollectionSavePacket[0].ModifiedColumns);
                        esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                        DataRow[] singleRow = new DataRow[1];

                        // Delete each record
                        foreach (esEntitySavePacket packet in request.CollectionSavePacket)
                        {
                            DataRow row = dataTable.NewRow();
                            dataTable.Rows.Add(row);

                            SetOriginalValues(request, packet, row, true);
                            row.AcceptChanges();
                            row.Delete();

                            singleRow[0] = row;

                            #region Profiling
                            if (sTraceHandler != null)
                            {
                                using (esTraceArguments esTrace = new esTraceArguments(request, cmd, packet, "SaveCollectionDynamic", System.Environment.StackTrace))
                                {
                                    try
                                    {
                                        da.Update(singleRow);
                                    }
                                    catch (Exception ex)
                                    {
                                        esTrace.Exception = ex.Message;
                                        throw;
                                    }
                                }
                            }
                            else
                            #endregion
                            {
                                da.Update(singleRow);
                            }

                            if (row.HasErrors)
                            {
                                request.FireOnError(packet, row.RowError);
                            }

                            dataTable.Rows.Clear(); // ADO.NET won't let us reuse the same DataRow
                        }
                    }
                    finally
                    {
                        esTransactionScope.DeEnlist(cmd);
                    }
                }
                scope.Complete();
            }

            return request.Table;
        }

        static private DataTable SaveDynamicEntity(esDataRequest request)
        {
            bool needToDelete = request.EntitySavePacket.RowState == esDataRowState.Deleted;

            DataTable dataTable = CreateDataTable(request);

            using (MySqlDataAdapter da = new MySqlDataAdapter())
            {
                da.AcceptChangesDuringUpdate = false;

                DataRow row = dataTable.NewRow();
                dataTable.Rows.Add(row);

                MySqlCommand cmd = null;

                switch (request.EntitySavePacket.RowState)
                {
                    case esDataRowState.Added:
                        cmd = da.InsertCommand = Shared.BuildDynamicInsertCommand(request, request.EntitySavePacket.ModifiedColumns);
                        SetModifiedValues(request, request.EntitySavePacket, row);
                        break;

                    case esDataRowState.Modified:
                        cmd = da.UpdateCommand = Shared.BuildDynamicUpdateCommand(request, request.EntitySavePacket.ModifiedColumns);
                        SetOriginalValues(request, request.EntitySavePacket, row, false);
                        SetModifiedValues(request, request.EntitySavePacket, row);
                        row.AcceptChanges();
                        row.SetModified();
                        break;

                    case esDataRowState.Deleted:
                        cmd = da.DeleteCommand = Shared.BuildDynamicDeleteCommand(request, null);
                        SetOriginalValues(request, request.EntitySavePacket, row, true);
                        row.AcceptChanges();
                        row.Delete();
                        break;
                }

                if (!needToDelete && request.Properties != null)
                {
                    request.Properties["esDataRequest"] = request;
                    request.Properties["esEntityData"] = request.EntitySavePacket;
                    dataTable.ExtendedProperties["props"] = request.Properties;
                }

                DataRow[] singleRow = new DataRow[1];
                singleRow[0] = row;

                if (!request.IgnoreComputedColumns)
                {
                    da.RowUpdated += new MySqlRowUpdatedEventHandler(OnRowUpdated);
                }

                try
                {
                    esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, request.EntitySavePacket, "SaveEntityDynamic", System.Environment.StackTrace))
                        {
                            try
                            {
                                da.Update(singleRow);
                            }
                            catch (Exception ex)
                            {
                                esTrace.Exception = ex.Message;
                                throw;
                            }
                        }
                    }
                    else
                    #endregion
                    {
                        da.Update(singleRow);
                    }
                }
                finally
                {
                    // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                    esTransactionScope.DeEnlist(cmd);
                }

                // [C2 FIX — complement] Sync keys BEFORE the framework calls AcceptChanges.
                SyncColumnAndPropertyKeys(request.EntitySavePacket, request.Columns);

                if (request.EntitySavePacket.RowState != esDataRowState.Deleted && cmd.Parameters != null)
                {
                    foreach (MySqlParameter param in cmd.Parameters)
                    {
                        switch (param.Direction)
                        {
                            case ParameterDirection.Output:
                            case ParameterDirection.InputOutput:
                                request.EntitySavePacket.CurrentValues[param.SourceColumn] = param.Value;
                                break;
                        }
                    }
                }
            }

            return dataTable;
        }

        static private DataTable CreateDataTable(esDataRequest request)
        {
            DataTable dataTable = new DataTable();
            DataColumnCollection dataColumns = dataTable.Columns;
            esColumnMetadataCollection cols = request.Columns;

            if (request.SelectedColumns == null)
            {
                esColumnMetadata col;
                for (int i = 0; i < cols.Count; i++)
                {
                    col = cols[i];
                    dataColumns.Add(new DataColumn(col.Name, col.Type));
                }
            }
            else
            {
                foreach (string col in request.SelectedColumns.Keys)
                {
                    dataColumns.Add(new DataColumn(col, cols[col].Type));
                }
            }

            return dataTable;
        }

        // ===================================================================
        // [C1/C2 FIX] Tolerant value lookup: accepts both the DB column name
        // ("invoiceid") and the EntitySpaces property name ("Invoiceid").
        // ApplyPostSaveKeys writes CurrentValues[PropertyName] on snake_case
        // providers (or lowercase column providers like this Northwind), so
        // the naive FindByColumnName lookup fails silently.
        // ===================================================================
        static private esColumnMetadata FindColumnByAnyName(esColumnMetadataCollection columns, string name)
        {
            if (columns == null || string.IsNullOrEmpty(name)) return null;

            var byColumn = columns.FindByColumnName(name);
            if (byColumn != null) return byColumn;

            foreach (esColumnMetadata col in columns)
            {
                if (string.Equals(col.PropertyName, name, StringComparison.Ordinal))
                    return col;
            }
            return null;
        }

        static private object GetValueFromDictionary(esSmartDictionary dict, esColumnMetadata col)
        {
            if (dict == null || col == null) return null;

            if (dict.ContainsKey(col.Name))
            {
                object v = dict[col.Name];
                if (v != null && v != DBNull.Value) return v;
            }

            if (!string.IsNullOrEmpty(col.PropertyName) && dict.ContainsKey(col.PropertyName))
            {
                object v = dict[col.PropertyName];
                if (v != null && v != DBNull.Value) return v;
            }

            return null;
        }

        static void SetOriginalValues(esDataRequest request, esEntitySavePacket packet, DataRow row, bool primaryKeysAndConcurrencyOnly)
        {
            foreach (esColumnMetadata col in request.Columns)
            {
                if (primaryKeysAndConcurrencyOnly &&
                    (!col.IsInPrimaryKey && !col.IsConcurrency && !col.IsEntitySpacesConcurrency)) continue;

                object value = GetValueFromDictionary(packet.OriginalValues, col);
                if (value != null)
                    row[col.Name] = value;
            }
        }

        static void SetModifiedValues(esDataRequest request, esEntitySavePacket packet, DataRow row)
        {
            foreach (string key in packet.ModifiedColumns)
            {
                esColumnMetadata col = FindColumnByAnyName(request.Columns, key);
                if (col == null) continue;

                object value = GetValueFromDictionary(packet.CurrentValues, col);
                row[col.Name] = value ?? (object)DBNull.Value;
            }
        }

        // ===================================================================
        // [C2 FIX — complement] After a successful INSERT/UPDATE, synchronize
        // CurrentValues keys so both the DB column name ("invoiceid") and the
        // EntitySpaces property name ("Invoiceid") hold the same value.
        //
        // Why: ApplyPostSaveKeys writes the FK under the PROPERTY name via
        // SetProperty("Invoiceid", value). The entity getter, however, reads
        // under the COLUMN name via GetSystemInt32("invoiceid"). Without this
        // sync, the FK is correctly persisted to the DB (thanks to the C1 fix)
        // but stays invisible to the in-memory entity (getter returns null).
        //
        // Direction is bidirectional: any key with a real value is propagated
        // to its alias if the alias slot is empty.
        // ===================================================================
        static private void SyncColumnAndPropertyKeys(esEntitySavePacket packet, esColumnMetadataCollection columns)
        {
            if (packet.CurrentValues == null || columns == null) return;

            foreach (esColumnMetadata col in columns)
            {
                if (string.IsNullOrEmpty(col.PropertyName)) continue;
                if (string.Equals(col.Name, col.PropertyName, StringComparison.Ordinal)) continue;

                // Property → Column
                if (packet.CurrentValues.ContainsKey(col.PropertyName))
                {
                    object propVal = packet.CurrentValues[col.PropertyName];
                    if (propVal != null && propVal != DBNull.Value)
                    {
                        object colVal = packet.CurrentValues.ContainsKey(col.Name)
                            ? packet.CurrentValues[col.Name] : null;
                        if (colVal == null || colVal == DBNull.Value)
                            packet.CurrentValues[col.Name] = propVal;
                    }
                }

                // Column → Property
                if (packet.CurrentValues.ContainsKey(col.Name))
                {
                    object colVal = packet.CurrentValues[col.Name];
                    if (colVal != null && colVal != DBNull.Value)
                    {
                        object propVal = packet.CurrentValues.ContainsKey(col.PropertyName)
                            ? packet.CurrentValues[col.PropertyName] : null;
                        if (propVal == null || propVal == DBNull.Value)
                            packet.CurrentValues[col.PropertyName] = colVal;
                    }
                }
            }
        }


        protected static void OnRowUpdated(object sender, MySqlRowUpdatedEventArgs e)
        {
            try
            {
                PropertyCollection props = e.Row.Table.ExtendedProperties;
                if (props.ContainsKey("props"))
                {
                    props = (PropertyCollection)props["props"];
                }

                if (e.Status != UpdateStatus.Continue) return;
                if (e.StatementType != StatementType.Insert && e.StatementType != StatementType.Update) return;

                esDataRequest request = props["esDataRequest"] as esDataRequest;
                esEntitySavePacket packet = (esEntitySavePacket)props["esEntityData"];
                string source = props["Source"] as string;

                if (e.StatementType == StatementType.Insert)
                {
                    if (props.Contains("AutoInc"))
                    {
                        string autoIncColumn = props["AutoInc"] as string;

                        MySqlCommand cmd = new MySqlCommand();
                        cmd.Connection = e.Command.Connection;
                        cmd.Transaction = e.Command.Transaction;
                        cmd.CommandText = "SELECT LAST_INSERT_ID();";

                        object o = null;

                        #region Profiling
                        if (sTraceHandler != null)
                        {
                            using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "OnRowUpdated", System.Environment.StackTrace))
                            {
                                try { o = cmd.ExecuteScalar(); }
                                catch (Exception ex) { esTrace.Exception = ex.Message; throw; }
                            }
                        }
                        else
                        #endregion
                        {
                            o = cmd.ExecuteScalar();
                        }

                        if (o != null && o != DBNull.Value)
                        {
                            // [C8 FIX] Write to the DataRow by column name.
                            e.Row[autoIncColumn] = o;

                            // [C8 FIX — hallazgo] Parameter name is "?PropertyName", not "?ColumnName".
                            // Iterate by SourceColumn (which IS the column name) to find the right param.
                            foreach (MySqlParameter p in e.Command.Parameters)
                            {
                                if (string.Equals(p.SourceColumn, autoIncColumn, StringComparison.OrdinalIgnoreCase))
                                {
                                    p.Value = o;
                                    break;
                                }
                            }

                            // Also update CurrentValues under both keys so getters see the value.
                            if (packet.CurrentValues != null)
                            {
                                packet.CurrentValues[autoIncColumn] = o;

                                esColumnMetadata meta = request?.Columns?.FindByColumnName(autoIncColumn);
                                if (meta != null && !string.IsNullOrEmpty(meta.PropertyName)
                                    && !string.Equals(meta.PropertyName, autoIncColumn, StringComparison.Ordinal))
                                {
                                    packet.CurrentValues[meta.PropertyName] = o;
                                }
                            }
                        }
                    }

                    if (props.Contains("EntitySpacesConcurrency"))
                    {
                        string esConcurrencyColumn = props["EntitySpacesConcurrency"] as string;
                        if (packet.CurrentValues != null)
                            packet.CurrentValues[esConcurrencyColumn] = 1;
                    }
                }

                if (props.Contains("Timestamp"))
                {
                    string column = props["Timestamp"] as string;

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = e.Command.Connection;
                    cmd.Transaction = e.Command.Transaction;
                    cmd.CommandText = "SELECT LastTimestamp('" + source + "');";

                    object o = null;

                    #region Profiling
                    if (sTraceHandler != null)
                    {
                        using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "OnRowUpdated", System.Environment.StackTrace))
                        {
                            try { o = cmd.ExecuteScalar(); }
                            catch (Exception ex) { esTrace.Exception = ex.Message; throw; }
                        }
                    }
                    else
                    #endregion
                    {
                        o = cmd.ExecuteScalar();
                    }

                    if (o != null && o != DBNull.Value)
                    {
                        foreach (MySqlParameter p in e.Command.Parameters)
                        {
                            if (string.Equals(p.SourceColumn, column, StringComparison.OrdinalIgnoreCase))
                            {
                                p.Value = o;
                                break;
                            }
                        }
                    }
                }

                if (props.Contains("Defaults"))
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = e.Command.Connection;
                    cmd.Transaction = e.Command.Transaction;

                    string select = (string)props["Defaults"];
                    string[] whereParameters = ((string)props["Where"]).Split(',');

                    string comma = String.Empty;
                    string where = String.Empty;
                    int i = 1;
                    foreach (string parameter in whereParameters)
                    {
                        MySqlParameter p = new MySqlParameter("?p" + i++.ToString(), e.Row[parameter]);
                        cmd.Parameters.Add(p);
                        where += comma + "`" + parameter + "` = " + p.ParameterName;
                        comma = " AND ";
                    }

                    cmd.CommandText = "SELECT " + select + " FROM `" + request.ProviderMetadata.Source + "` WHERE " + where + ";";

                    MySqlDataReader rdr = null;
                    try
                    {
                        #region Profiling
                        if (sTraceHandler != null)
                        {
                            using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "OnRowUpdated", System.Environment.StackTrace))
                            {
                                try { rdr = cmd.ExecuteReader(CommandBehavior.SingleResult); }
                                catch (Exception ex) { esTrace.Exception = ex.Message; throw; }
                            }
                        }
                        else
                        #endregion
                        {
                            rdr = cmd.ExecuteReader(CommandBehavior.SingleResult);
                        }

                        if (rdr.Read())
                        {
                            string cleanSelect = select.Replace("`", String.Empty);
                            string[] selectCols = cleanSelect.Split(',');

                            for (int k = 0; k < selectCols.Length && k < rdr.FieldCount; k++)
                            {
                                string key = selectCols[k].Trim();
                                object val = rdr.IsDBNull(k) ? null : rdr.GetValue(k);

                                if (packet.CurrentValues != null)
                                {
                                    packet.CurrentValues[key] = val;

                                    esColumnMetadata meta = request?.Columns?.FindByColumnName(key);
                                    if (meta != null && !string.IsNullOrEmpty(meta.PropertyName))
                                        packet.CurrentValues[meta.PropertyName] = val;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (rdr != null) rdr.Close();
                    }
                }

                if (e.StatementType == StatementType.Update && props.Contains("EntitySpacesConcurrency"))
                {
                    string colName = props["EntitySpacesConcurrency"] as string;
                    object o = e.Row[colName];
                    if (o != null && o != DBNull.Value)
                    {
                        foreach (MySqlParameter p in e.Command.Parameters)
                        {
                            if (string.Equals(p.SourceColumn, colName, StringComparison.OrdinalIgnoreCase))
                            {
                                object v = null;
                                switch (Type.GetTypeCode(o.GetType()))
                                {
                                    case TypeCode.Int16: v = ((System.Int16)o) + 1; break;
                                    case TypeCode.Int32: v = ((System.Int32)o) + 1; break;
                                    case TypeCode.Int64: v = ((System.Int64)o) + 1; break;
                                    case TypeCode.UInt16: v = ((System.UInt16)o) + 1; break;
                                    case TypeCode.UInt32: v = ((System.UInt32)o) + 1; break;
                                    case TypeCode.UInt64: v = ((System.UInt64)o) + 1; break;
                                }
                                p.Value = v;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // [C8 FIX — enhanced] Surface the failure so it is not silently
                // swallowed. Two complementary mechanisms:
                //
                //   1. Mark the DataRow with RowError. The caller's
                //      ContinueUpdateOnError logic iterates GetErrors() and
                //      calls request.FireOnError(packet, row.RowError), which
                //      sets entity.rowError and is visible to application code.
                //
                //   2. Emit the exception through both Debug (visible in IDE
                //      during development) and Trace (visible in production
                //      when a trace listener is attached — ETW, log4net, etc.).
                //
                // Do NOT change e.Status to UpdateStatus.ErrorsOccurred here:
                // the SQL statement already executed successfully. What failed
                // is the post-processing (LAST_INSERT_ID, LastTimestamp, or
                // defaults retrieval). Failing the whole batch would discard a
                // successful insert, which is worse than surfacing the missing
                // value as a row error.

                try
                {
                    if (string.IsNullOrEmpty(e.Row.RowError))
                    {
                        e.Row.RowError = "OnRowUpdated: " + ex.Message;
                    }
                }
                catch { /* never let error-surfacing itself throw */ }

                System.Diagnostics.Debug.WriteLine(
                    "[EntitySpaces.MySQLProvider] OnRowUpdated failed: " + ex);

                System.Diagnostics.Trace.WriteLine(
                    "[EntitySpaces.MySQLProvider] OnRowUpdated failed: " + ex);
            }
        }
    }
}

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

using EntitySpaces.DynamicQuery;
using EntitySpaces.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace EntitySpaces.Npgsql2Provider
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

            public esTraceArguments(esDataRequest request, IDbCommand cmd, string action, string callStack)
            {
                PacketOrder = Interlocked.Increment(ref esTraceArguments.packetOrder);

                this.command = cmd;

                TraceChannel = DataProvider.sTraceChannel;
                Syntax = "POSTGRESQL";
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
                        NpgsqlParameter param = parameters[i] as NpgsqlParameter;

                        esTraceParameter p = new esTraceParameter()
                        {
                            Name = param.ParameterName,
                            Direction = param.Direction.ToString(),
                            ParamType = param.NpgsqlDbType.ToString().ToUpper(),
                            BeforeValue = param.Value != null ? Convert.ToString(param.Value) : "null"
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
            return new NpgsqlConnection();
        }

        static private void CleanupCommand(NpgsqlCommand cmd)
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
                        NpgsqlCommand cmd = QueryBuilder.PrepareCommand(request);
                        LoadDataTableFromDynamicQuery(request, response, cmd);
                        break;

                    case esQueryType.DynamicQueryParseOnly:

                        response = new esDataResponse();
                        NpgsqlCommand cmd1 = QueryBuilder.PrepareCommand(request);
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
            catch (NpgsqlException ex)
            {
                esException es = Shared.CheckForConcurrencyException(ex);
                if (es != null)
                    response.Exception = es;
                else
                    response.Exception = ex;
            }
            catch (DBConcurrencyException dbex)
            {
                response.Exception = new esConcurrencyException("Error in SqlClientProvider.esSaveDataTable", dbex);
            }

            response.Table = request.Table;
            return response;
        }

        esDataResponse IDataProvider.ExecuteNonQuery(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                cmd = new NpgsqlCommand();
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
                        cmd.CommandText = Shared.CreateFullName(request);
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
                    // [REVISED] No explicit ROLLBACK. The connection may be enlisted in
                    // an ambient TransactionScope (test harness, ES transaction scope);
                    // issuing ROLLBACK would abort the AMBIENT transaction, not just this
                    // command. System.Transactions handles rollback when the scope ends.
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
            NpgsqlCommand cmd = null;

            try
            {
                cmd = new NpgsqlCommand();
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
                        cmd.CommandText = Shared.CreateFullName(request);
                        break;

                    case esQueryType.Text:
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = request.QueryText;
                        break;

                    case esQueryType.DynamicQuery:
                        cmd = QueryBuilder.PrepareCommand(request);
                        break;
                }

                cmd.Connection = new NpgsqlConnection(request.ConnectionString);
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
            catch (Exception ex)
            {
                // [REVISED] Close the connection cleanly on error. No ROLLBACK is
                // issued — PostgreSQL rolls back any pending transaction automatically
                // when the connection is closed. If the connection was enlisted in an
                // ambient scope, the scope owner handles the rollback.
                CleanupCommand(cmd);
                response.Exception = ex;
            }

            return response;
        }

        esDataResponse IDataProvider.ExecuteScalar(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                cmd = new NpgsqlCommand();
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
                        cmd.CommandText = Shared.CreateFullName(request);
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
            NpgsqlCommand cmd = null;

            try
            {
                DataSet dataSet = new DataSet();

                cmd = new NpgsqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Shared.CreateFullName(request);

                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.DataSet = dataSet;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }

            return response;
        }

        static private esDataResponse LoadDataSetFromText(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                DataSet dataSet = new DataSet();

                cmd = new NpgsqlCommand();
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.DataSet = dataSet;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }

            return response;
        }

        static private esDataResponse LoadDataTableFromStoredProcedure(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new NpgsqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = Shared.CreateFullName(request);
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }

            return response;
        }

        static private esDataResponse LoadDataTableFromText(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new NpgsqlCommand();
                cmd.CommandType = CommandType.Text;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;
                if (request.Parameters != null) Shared.AddParameters(cmd, request);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;

                if (request.Parameters != null)
                {
                    Shared.GatherReturnParameters(cmd, request, response);
                }
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }

            return response;
        }

        static private esDataResponse LoadManyToMany(esDataRequest request)
        {
            esDataResponse response = new esDataResponse();
            NpgsqlCommand cmd = null;

            try
            {
                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                cmd = new NpgsqlCommand();
                cmd.CommandType = CommandType.Text;
                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

                string mmQuery = request.QueryText;

                string[] sections = mmQuery.Split('|');
                string[] tables = sections[0].Split(',');
                string[] columns = sections[1].Split(',');

                string prefix = String.Empty;

                if (request.Catalog != null || request.ProviderMetadata.Catalog != null)
                {
                    prefix += Delimiters.TableOpen;
                    prefix += request.Catalog != null ? request.Catalog : request.ProviderMetadata.Catalog;
                    prefix += Delimiters.TableClose + ".";
                }

                if (request.Schema != null || request.ProviderMetadata.Schema != null)
                {
                    prefix += Delimiters.TableOpen;
                    prefix += request.Schema != null ? request.Schema : request.ProviderMetadata.Schema;
                    prefix += Delimiters.TableClose + ".";
                }

                string table0 = prefix + Delimiters.TableOpen + tables[0] + Delimiters.TableClose;
                string table1 = prefix + Delimiters.TableOpen + tables[1] + Delimiters.TableClose;

                string sql = "SELECT * FROM " + table0 + " JOIN " + table1 + " ON " + table0 + ".\"" + columns[0] + "\" = ";
                sql += table1 + ".\"" + columns[1] + "\" WHERE " + table1 + ".\"" + sections[2] + "\" = @";

                if (request.Parameters != null)
                {
                    foreach (esParameter esParam in request.Parameters)
                    {
                        sql += esParam.Name;
                    }

                    Shared.AddParameters(cmd, request);
                }

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }

            return response;
        }

        // This is used only to execute the Dynamic Query API
        static private void LoadDataTableFromDynamicQuery(esDataRequest request, esDataResponse response, NpgsqlCommand cmd)
        {
            try
            {
                response.LastQuery = cmd.CommandText;

                if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

                DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

                NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
                    // [REVISED] No explicit ROLLBACK.
                    esTransactionScope.DeEnlist(da.SelectCommand);
                }

                response.Table = dataTable;
            }
            catch (Exception ex)
            {
                CleanupCommand(cmd);
                throw ex;
            }
        }

        // This is used only to execute the Dynamic Query API
        //static private void LoadDataTableForLinqToSql(esDataRequest request, esDataResponse response)
        //{
        //    NpgsqlCommand cmd = null;

        //    try
        //    {
        //        DataTable dataTable = new DataTable(request.ProviderMetadata.Destination);

        //        cmd = request.LinqContext.GetCommand(request.LinqQuery) as NpgsqlCommand;

        //        response.LastQuery = cmd.CommandText;

        //        if (request.CommandTimeout != null) cmd.CommandTimeout = request.CommandTimeout.Value;

        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter();
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
        //    catch (Exception)
        //    {
        //        CleanupCommand(cmd);
        //        throw;
        //    }
        //    finally
        //    {

        //    }
        //}

        static private DataTable SaveStoredProcCollection(esDataRequest request)
        {
            if (request.CollectionSavePacket == null) return null;

            NpgsqlCommand cmdInsert = null;
            NpgsqlCommand cmdUpdate = null;
            NpgsqlCommand cmdDelete = null;

            try
            {
                using (esTransactionScope scope = new esTransactionScope())
                {
                    NpgsqlCommand cmd = null;
                    bool exception = false;

                    foreach (esEntitySavePacket packet in request.CollectionSavePacket)
                    {
                        cmd = null;
                        exception = false;

                        #region Setup Commands
                        switch (packet.RowState)
                        {
                            case esDataRowState.Added:
                                if (cmdInsert == null)
                                {
                                    cmdInsert = Shared.BuildStoredProcInsertCommand(request, packet);
                                    esTransactionScope.Enlist(cmdInsert, request.ConnectionString, CreateIDbConnectionDelegate);
                                }
                                cmd = cmdInsert;
                                break;
                            case esDataRowState.Modified:
                                if (cmdUpdate == null)
                                {
                                    cmdUpdate = Shared.BuildStoredProcUpdateCommand(request, packet);
                                    esTransactionScope.Enlist(cmdUpdate, request.ConnectionString, CreateIDbConnectionDelegate);
                                }
                                cmd = cmdUpdate;
                                break;
                            case esDataRowState.Deleted:
                                if (cmdDelete == null)
                                {
                                    cmdDelete = Shared.BuildStoredProcDeleteCommand(request, packet);
                                    esTransactionScope.Enlist(cmdDelete, request.ConnectionString, CreateIDbConnectionDelegate);
                                }
                                cmd = cmdDelete;
                                break;

                            case esDataRowState.Unchanged:
                                continue;
                        }
                        #endregion

                        #region Preprocess Parameters
                        if (cmd.Parameters != null)
                        {
                            foreach (NpgsqlParameter param in cmd.Parameters)
                            {
                                if (param.Direction == ParameterDirection.Output)
                                {
                                    param.Value = null;
                                }
                                else
                                {
                                    if (packet.CurrentValues.ContainsKey(param.SourceColumn))
                                    {
                                        param.Value = packet.CurrentValues[param.SourceColumn];
                                    }
                                    else
                                    {
                                        param.Value = null;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Execute Command
                        try
                        {
                            int count;

                            #region Profiling
                            if (sTraceHandler != null)
                            {
                                using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveCollectionStoredProcedure", System.Environment.StackTrace))
                                {
                                    try
                                    {
                                        count = cmd.ExecuteNonQuery();
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
                                count = cmd.ExecuteNonQuery();
                            }

                            if (count < 1)
                            {
                                throw new esConcurrencyException("Update failed to update any records");
                            }
                        }
                        catch (Exception ex)
                        {
                            exception = true;
                            request.FireOnError(packet, ex.Message);
                            if (!request.ContinueUpdateOnError)
                            {
                                throw;
                            }
                        }
                        #endregion

                        #region Postprocess Parameters
                        if (!exception && packet.RowState != esDataRowState.Deleted && cmd.Parameters != null)
                        {
                            foreach (NpgsqlParameter param in cmd.Parameters)
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
                        #endregion

                        // [REVISED] No per-packet ROLLBACK — see ExecuteNonQuery for rationale.
                    }

                    scope.Complete();
                }
            }
            finally
            {
                if (cmdInsert != null) esTransactionScope.DeEnlist(cmdInsert);
                if (cmdUpdate != null) esTransactionScope.DeEnlist(cmdUpdate);
                if (cmdDelete != null) esTransactionScope.DeEnlist(cmdDelete);
            }

            return null;
        }

        static private DataTable SaveStoredProcEntity(esDataRequest request)
        {
            NpgsqlCommand cmd = null;

            switch (request.EntitySavePacket.RowState)
            {
                case esDataRowState.Added:
                    cmd = Shared.BuildStoredProcInsertCommand(request, request.EntitySavePacket);
                    break;

                case esDataRowState.Modified:
                    cmd = Shared.BuildStoredProcUpdateCommand(request, request.EntitySavePacket);
                    break;

                case esDataRowState.Deleted:
                    cmd = Shared.BuildStoredProcDeleteCommand(request, request.EntitySavePacket);
                    break;

                case esDataRowState.Unchanged:
                    return null;
            }

            try
            {
                esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);
                int count = 0;

                #region Profiling
                if (sTraceHandler != null)
                {
                    using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveEntityStoredProcedure", System.Environment.StackTrace))
                    {
                        try
                        {
                            count = cmd.ExecuteNonQuery();
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
                    count = cmd.ExecuteNonQuery();
                }

                if (count < 1)
                {
                    throw new esConcurrencyException("Update failed to update any records");
                }
            }
            finally
            {
                // [REVISED] No explicit ROLLBACK.
                esTransactionScope.DeEnlist(cmd);
                cmd.Dispose();
            }

            if (request.EntitySavePacket.RowState != esDataRowState.Deleted && cmd.Parameters != null)
            {
                foreach (NpgsqlParameter param in cmd.Parameters)
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

            return null;
        }

        static private DataTable SaveDynamicCollection(esDataRequest request)
        {
            if (request.CollectionSavePacket == null) return null;

            using (esTransactionScope scope = new esTransactionScope())
            {
                foreach (esEntitySavePacket packet in request.CollectionSavePacket)
                {
                    NpgsqlCommand cmd = null;

                    switch (packet.RowState)
                    {
                        case esDataRowState.Added:
                            cmd = Shared.BuildDynamicInsertCommand(request, packet);
                            break;
                        case esDataRowState.Modified:
                            cmd = Shared.BuildDynamicUpdateCommand(request, packet);
                            break;
                        case esDataRowState.Deleted:
                            cmd = Shared.BuildDynamicDeleteCommand(request, packet);
                            break;
                        case esDataRowState.Unchanged:
                            continue;
                    }

                    try
                    {
                        esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);
                        // [REVISED] EnsureConnectionHealthy() removed — its ROLLBACK
                        // branch could abort the ambient transaction.

                        int count = 0;

                        #region Profiling
                        if (sTraceHandler != null)
                        {
                            using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveCollectionDynamic", System.Environment.StackTrace))
                            {
                                try
                                {
                                    count = ExecuteInsertCommand(cmd, packet);
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
                            count = ExecuteInsertCommand(cmd, packet);
                        }

                        if (packet.RowState != esDataRowState.Deleted && cmd?.Parameters != null)
                        {
                            MapOutputParameters(cmd.Parameters, packet.CurrentValues, request.Columns);
                        }

                        if (count < 1)
                            throw new esConcurrencyException("Update failed to update any records");
                    }
                    catch (NpgsqlException ex)
                    {
                        esConcurrencyException ce = Shared.CheckForConcurrencyException(ex);
                        if (ce != null)
                        {
                            request.FireOnError(packet, ce.Message);
                            if (!request.ContinueUpdateOnError) throw ce;
                        }
                        else
                        {
                            request.FireOnError(packet, ex.Message);
                            if (!request.ContinueUpdateOnError) throw;
                        }
                    }
                    catch (Exception ex)
                    {
                        request.FireOnError(packet, ex.Message);
                        if (!request.ContinueUpdateOnError) throw;
                    }
                    finally
                    {
                        // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                        esTransactionScope.DeEnlist(cmd);
                        cmd?.Dispose();
                    }
                }

                scope.Complete();
            }

            return null;
        }

        static private DataTable SaveDynamicEntity(esDataRequest request)
        {
            NpgsqlCommand cmd = null;

            switch (request.EntitySavePacket.RowState)
            {
                case esDataRowState.Added:
                    cmd = Shared.BuildDynamicInsertCommand(request, request.EntitySavePacket);
                    break;
                case esDataRowState.Modified:
                    cmd = Shared.BuildDynamicUpdateCommand(request, request.EntitySavePacket);
                    break;
                case esDataRowState.Deleted:
                    cmd = Shared.BuildDynamicDeleteCommand(request, request.EntitySavePacket);
                    break;
            }

            try
            {
                esTransactionScope.Enlist(cmd, request.ConnectionString, CreateIDbConnectionDelegate);
                // [REVISED] EnsureConnectionHealthy() removed — its ROLLBACK branch
                // could abort the ambient transaction.

                int count = 0;

                #region Profiling
                if (sTraceHandler != null)
                {
                    using (esTraceArguments esTrace = new esTraceArguments(request, cmd, "SaveEntityDynamic", System.Environment.StackTrace))
                    {
                        try
                        {
                            count = ExecuteInsertCommand(cmd, request.EntitySavePacket);
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
                    count = ExecuteInsertCommand(cmd, request.EntitySavePacket);
                }

                if (request.EntitySavePacket.RowState != esDataRowState.Deleted && cmd?.Parameters != null)
                {
                    MapOutputParameters(cmd.Parameters, request.EntitySavePacket.CurrentValues, request.Columns);
                }

                if (count < 1)
                    throw new esConcurrencyException("Update failed to update any records");
            }
            catch (NpgsqlException ex)
            {
                esConcurrencyException ce = Shared.CheckForConcurrencyException(ex);
                if (ce != null) throw ce;
                else throw;
            }
            finally
            {
                // [REVISED] No explicit ROLLBACK — see ExecuteNonQuery for rationale.
                esTransactionScope.DeEnlist(cmd);
                cmd?.Dispose();
            }

            return null;
        }

        // ===================================================================
        // Executes an INSERT that may carry a RETURNING clause.
        //
        // [NEW ADJUSTMENT] Key fixes vs. the previous version:
        //   1. Uses CommandBehavior.SingleRow so the reader closes as soon as
        //      the first row is consumed, releasing the connection cleanly and
        //      leaving no active reader on the enlisted connection.
        //   2. Does NOT loop over NextResult() — a single result set is
        //      present, and consuming it unnecessarily was the root cause of
        //      the SQLSTATE 25P02 cascade during hierarchical saves.
        //   3. Uses IsDBNull() before reading so NULL columns are mapped
        //      cleanly instead of throwing.
        //   4. Reports mapping failures through Debug.WriteLine instead of
        //      swallowing them, so integration issues (wrong column name,
        //      missing column in RETURNING) become visible in tests.
        // ===================================================================
        private static int ExecuteInsertCommand(NpgsqlCommand cmd, esEntitySavePacket packet)
        {
            bool hasReturning = cmd.CommandText.IndexOf("RETURNING", StringComparison.OrdinalIgnoreCase) >= 0;

            if (!hasReturning)
                return cmd.ExecuteNonQuery();

            int rows = 0;

            // [NEW ADJUSTMENT] SingleRow + using ensures the connection is
            // released cleanly, avoiding the aborted-transaction cascade.
            using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    rows = 1;

                    foreach (NpgsqlParameter p in cmd.Parameters)
                    {
                        if (p.Direction != ParameterDirection.Output &&
                            p.Direction != ParameterDirection.InputOutput)
                            continue;

                        // SourceColumn is populated by Cache.GetParameters with the
                        // canonical DB column name; fall back to the parameter name.
                        string colName = !string.IsNullOrEmpty(p.SourceColumn)
                            ? p.SourceColumn
                            : p.ParameterName.TrimStart(':', '@');

                        try
                        {
                            int ordinal = reader.GetOrdinal(colName);
                            p.Value = reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Column not present in RETURNING — expected for non-returned
                            // columns such as concurrency tokens that were not requested.
                        }
                        catch (Exception ex)
                        {
                            // [NEW ADJUSTMENT] Surface unexpected mapping failures so
                            // they are not silently swallowed as in the previous code.
                            System.Diagnostics.Debug.WriteLine(
                                $"[EntitySpaces.Npgsql2Provider] Failed to map RETURNING column '{colName}': {ex.Message}");
                        }
                    }
                }
            }

            // RETURNING always reports -1 as RecordsAffected; treat a mapped row
            // as a successful insert of exactly one row.
            return rows;
        }

        // ===================================================================
        // Copies Output / InputOutput parameter values back to the entity's
        // CurrentValues dictionary, and synchronizes property-name keys to
        // column-name keys.
        //
        // [NEW ADJUSTMENT] Two responsibilities now:
        //
        //   1. Sync property → column keys. Generated ApplyPostSaveKeys calls
        //      SetProperty("OrderId", value) which stores the value under the
        //      property name. The entity's getters read by column name
        //      ("order_id"), so without this sync the value stays invisible to
        //      the rest of the framework. This affects every provider whose
        //      column and property names differ (PostgreSQL, Oracle, ...).
        //
        //   2. Map Output / InputOutput parameters (existing behaviour).
        // ===================================================================
        private static void MapOutputParameters(
            NpgsqlParameterCollection parameters,
            esSmartDictionary currentValues,
            esColumnMetadataCollection columns)
        {
            // -----------------------------------------------------------------
            // Step 1: sync property-name key → column-name key for any Input or
            // InputOutput parameter whose source column has a distinct property
            // name and whose column-name slot is empty.
            // -----------------------------------------------------------------
            if (columns != null)
            {
                foreach (NpgsqlParameter param in parameters)
                {
                    if (param.Direction != ParameterDirection.Input &&
                        param.Direction != ParameterDirection.InputOutput)
                        continue;

                    string colName = param.SourceColumn;
                    if (string.IsNullOrEmpty(colName)) continue;

                    esColumnMetadata meta = columns.FindByColumnName(colName);
                    if (meta == null || string.IsNullOrEmpty(meta.PropertyName)) continue;

                    // Nothing to sync when both names coincide (SQL Server).
                    if (string.Equals(colName, meta.PropertyName, StringComparison.Ordinal))
                        continue;

                    if (!currentValues.ContainsKey(meta.PropertyName))
                        continue;

                    object propVal = currentValues[meta.PropertyName];
                    if (propVal == null || propVal == DBNull.Value)
                        continue;

                    object colVal = currentValues.ContainsKey(colName)
                        ? currentValues[colName]
                        : null;

                    if (colVal == null || colVal == DBNull.Value)
                    {
                        currentValues[colName] = propVal;
                    }
                }
            }

            // -----------------------------------------------------------------
            // Step 2: existing Output / InputOutput mapping (unchanged).
            // -----------------------------------------------------------------
            foreach (NpgsqlParameter param in parameters)
            {
                if (param.Direction != ParameterDirection.Output &&
                    param.Direction != ParameterDirection.InputOutput)
                    continue;

                string colName = !string.IsNullOrEmpty(param.SourceColumn)
                    ? param.SourceColumn
                    : param.ParameterName.TrimStart(':', '@');

                string targetKey = null;
                string normalized = NormalizeKey(colName);

                foreach (string key in currentValues.Keys)
                {
                    if (NormalizeKey(key) == normalized)
                    {
                        targetKey = key;
                        break;
                    }
                }

                if (targetKey == null)
                    targetKey = colName;

                object value = param.Value;

                if (value is long longVal &&
                    longVal <= int.MaxValue && longVal >= int.MinValue)
                {
                    object existing = currentValues.ContainsKey(targetKey)
                        ? currentValues[targetKey]
                        : null;

                    if (existing == null || existing == DBNull.Value || existing is int)
                        value = (int)longVal;
                }

                currentValues[targetKey] = value;
            }
        }


        // ===================================================================
        // [NEW ADJUSTMENT] Normalizes a column key so PostgreSQL snake_case
        // names (e.g. "order_id") match EntitySpaces camelCase keys
        // (e.g. "OrderId"). Underscores are stripped and the result is
        // lower-cased before comparison.
        // ===================================================================
        private static string NormalizeKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return key.Replace("_", string.Empty).ToLowerInvariant();
        }


    } // end class
}

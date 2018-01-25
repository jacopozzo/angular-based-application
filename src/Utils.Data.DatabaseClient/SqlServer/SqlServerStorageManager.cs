using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Utils.Data.DatabaseClient.Abstractions;
using Utils.Extensions.Collections;

namespace Utils.Data.DatabaseClient.SqlServer
{
    public class SqlServerStorageManager : StorageManager
    {
        /// <summary>
        /// oggetto connessione
        /// </summary>
        private readonly SqlConnection _sqlConnection;

        /// <summary>
        /// oggetto Command
        /// </summary>
        private readonly SqlCommand _sqlCommand;

        private readonly IEnumerable<Type> _typesAllowed;

        /// <summary>
        /// oggetto Adapter per riempire il dataset
        /// </summary>
        private SqlDataAdapter _sqlDataAdapter;

        /// <summary>
        /// oggetto Transazione per gestire più richieste a db in maniera atomica
        /// </summary>
        private SqlTransaction _sqlTransaction;

        /// <summary>
        /// identifica una transazione atomica attiva
        /// </summary>
        private bool _transactionActive;

        public SqlServerStorageManager(string connectionString) : base(connectionString)
        {
            _sqlConnection = new SqlConnection(connectionString);
            _sqlCommand = new SqlCommand();
            _transactionActive = false;
            _typesAllowed = new[] { typeof(DataSet), typeof(DataTable), typeof(DataRow[]) };
        }

        /// <summary>
        /// </summary>
        /// <param name="isolationLevelString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IsolationLevel GetIsolationLevelFromString(string isolationLevelString)
        {
            if (isolationLevelString == null)
            {
                throw new ArgumentNullException(nameof(isolationLevelString));
            }

            if (String.IsNullOrEmpty(isolationLevelString))
            {
                return IsolationLevel.ReadCommitted;
            }
            return (IsolationLevel)Enum.Parse(typeof(IsolationLevel), isolationLevelString, true);
        }

        public static string GeneraComandoStored(string nomeStored, IEnumerable<IDbDataParameter> parameters)
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.AppendFormat("exec {0} \n", nomeStored);

            var sqlParameters = parameters as List<IDbDataParameter> ?? parameters.ToList();
            var lastParameter = sqlParameters.LastOrDefault();
            foreach (var parameter in sqlParameters)
            {
                try
                {
                    var sqlParameter = parameter as SqlParameter;
                    commandBuilder.AppendFormat("{0} = ", sqlParameter.ParameterName);
                    switch (sqlParameter.SqlDbType)
                    {
                        case SqlDbType.DateTime:
                            commandBuilder.AppendFormat("'{0}'", sqlParameter.SqlValue);
                            break;
                        case SqlDbType.NVarChar:
                        case SqlDbType.VarChar:
                            commandBuilder.AppendFormat("'{0}'", sqlParameter.Value);
                            break;
                        case SqlDbType.Int:
                            commandBuilder.Append(sqlParameter.Value);
                            break;
                        default:
                            commandBuilder.Append(sqlParameter.Value);
                            break;
                    }
                    if (sqlParameter != lastParameter)
                        commandBuilder.Append(",");
                    commandBuilder.AppendLine();
                }
                catch
                {
                }
            }
            {

            }
            return commandBuilder.ToString();
        }

        public override DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error, out Exception exception)
        {
            DataSet dataSet;
            error = string.Empty;
            exception = null;
            try
            {
                dataSet = new DataSet();
                _sqlCommand.CommandType = isStore ? CommandType.StoredProcedure : CommandType.Text;
                _sqlCommand.CommandText = storeProcedure;
                _sqlCommand.Connection = _sqlConnection;
                _sqlCommand.Parameters.Clear();
                if (parameters != null)
                {
                    foreach (var sqlParameter in parameters.Where(sqlParameter => sqlParameter != null))
                    {
                        _sqlCommand.Parameters.Add(sqlParameter);
                    }
                }
                _sqlDataAdapter = new SqlDataAdapter(_sqlCommand);
                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                    // Massimizzo il tempo di connessione dell'esecuzione su quella impostata nella stringa di connessione
                    _sqlCommand.CommandTimeout = _sqlConnection.ConnectionTimeout;
                }
                else
                {
                    _sqlCommand.Transaction = _sqlTransaction;
                    // Massimizzo il tempo di connessione dell'esecuzione su quella impostata nella transazione
                    _sqlCommand.CommandTimeout = _sqlTransaction.Connection.ConnectionTimeout;
                }
                _sqlDataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                exception = new Exception(
                    $"Errore nell'esecuzione della stored/query {storeProcedure}, parameters {(parameters != null ? parameters.Select(prm => $"paramName {prm.ParameterName}  - paramValue {prm.Value}").Stringify() : string.Empty)}, isStore {isStore}",
                  ex);
                dataSet = null;
            }
            finally
            {
                if (!_transactionActive)
                {
                    _sqlConnection.Close();
                }
            }
            return dataSet;
        }

        public override void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> parameters, bool isStored,
            out string error, out Exception exception)
        {
            if (!_typesAllowed.Contains(typeof(TDataSource)))
            {
                throw new ArgumentException("The data source types allowed are DataSet, DataTable, DataRow[]", nameof(dataSource));
            }
            error = string.Empty;
            exception = null;
            try
            {
                _sqlDataAdapter = new SqlDataAdapter
                {
                    UpdateCommand = new SqlCommand(update, _sqlConnection)
                };
                _sqlDataAdapter.UpdateCommand.CommandType = isStored ? CommandType.StoredProcedure : CommandType.Text;

                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                }
                else
                {
                    _sqlDataAdapter.UpdateCommand.Transaction = _sqlTransaction;
                }

                if (parameters != null)
                {
                    foreach (var sqlParameter in parameters.Where(sqlParameter => sqlParameter != null))
                    {
                        _sqlDataAdapter.UpdateCommand.Parameters.Add(sqlParameter);
                    }
                }

                var sqlAdapterType = _sqlDataAdapter.GetType();
                var methodInfo = sqlAdapterType.GetMethod("Update", new[] { typeof(TDataSource) });
                methodInfo.Invoke(_sqlDataAdapter, new object[] { dataSource });

                
            }
            finally
            {
                if (!_transactionActive)
                {
                    _sqlConnection.Close();
                }
            }
        }

        public override void Insert<TDataSource>(string insert, TDataSource dataSource, IEnumerable<IDbDataParameter> parameters)
        {
            if (insert == null)
            {
                throw new ArgumentNullException(nameof(insert));
            }
            if (!_typesAllowed.Contains(typeof(TDataSource)))
            {
                throw new ArgumentException("The data source types allowed are DataSet, DataTable, DataRow[]", nameof(dataSource));
            }
            try
            {
                _sqlDataAdapter = new SqlDataAdapter
                {
                    InsertCommand = new SqlCommand(insert, _sqlConnection)
                };
                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                }
                else
                {
                    _sqlDataAdapter.InsertCommand.Transaction = _sqlTransaction;
                }

                if (parameters != null)
                {
                    foreach (SqlParameter sqlParameter in parameters.Where(sqlParameter => sqlParameter != null))
                    {
                        _sqlDataAdapter.InsertCommand.Parameters.Add(sqlParameter);
                    }
                }

                var sqlAdapterType = _sqlDataAdapter.GetType();
                var methodInfo = sqlAdapterType.GetMethod("Update", new[] { typeof(TDataSource) });
                methodInfo.Invoke(_sqlDataAdapter, new object[] { dataSource });
            }
            catch (Exception)
            {
                if (_transactionActive)
                {
                    IsAllOkTransaction = false;
                }
                throw;
            }
            finally
            {
                if (!_transactionActive)
                {
                    _sqlConnection.Close();
                }
            }
        }


        public override void OpenTransaction(IsolationLevel? iso)
        {
            try
            {
                _sqlConnection.ConnectionString = ConnectionString;
                _sqlConnection.Open();
                _sqlTransaction = iso.HasValue ? _sqlConnection.BeginTransaction(iso.Value) : _sqlConnection.BeginTransaction();
                _transactionActive = true;
                IsAllOkTransaction = true;
            }
            catch (InvalidOperationException invalidOperation)
            {
                CloseTransaction(false);
                throw new Exception("Forzata la chiusura della connessione rimasta aperta", invalidOperation);
            }
        }

        public override void CloseTransaction(bool isAllOkTransaction)
        {
            if (isAllOkTransaction)
            {
                _sqlTransaction.Commit();
            }
            else
            {
                _sqlTransaction.Rollback();
            }
            if (_sqlConnection != null && _sqlConnection.State != ConnectionState.Closed)
            {
                _sqlConnection.Close();
            }

            _transactionActive = false;
        }

        public override DataTable ExecuteCommandOnDataTable(DataTable dataTable)
        {
            try
            {
                _sqlCommand.CommandType = CommandType.Text;
                _sqlCommand.CommandText = $"Select top 0 * from {dataTable.TableName} (nolock) ";
                _sqlCommand.Connection = _sqlConnection;
                _sqlCommand.Parameters.Clear();

                _sqlDataAdapter = new SqlDataAdapter(_sqlCommand);
                var sqlcomm = new SqlCommandBuilder(_sqlDataAdapter);

                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                }
                else
                {
                    _sqlCommand.Transaction = _sqlTransaction;
                }

                _sqlDataAdapter.Fill(dataTable);
                _sqlDataAdapter.Update(dataTable);

                /*this.sqlCommand.CommandText = "select scope_identity() as Id";
                this.sqlDataAdapter = new SqlDataAdapter(this.sqlCommand);
                var idDataTable = new DataTable();
                this.sqlDataAdapter.Fill(idDataTable); */


            }
            finally
            {
                if (!_transactionActive)
                    _sqlConnection.Close();
            }
            return dataTable;
        }

        public override void RollBack()
        {
            IsAllOkTransaction = false;
        }

        public override SqlCommand GenerateInsertCommand(DataTable table, bool insertedId = true)
        {
            try
            {
                _sqlCommand.CommandType = CommandType.Text;
                _sqlCommand.CommandText = $"Select top 0 * from {table.TableName} (nolock) ";
                _sqlCommand.Connection = _sqlConnection;
                _sqlCommand.Parameters.Clear();

                _sqlDataAdapter = new SqlDataAdapter(_sqlCommand);
                var sqlcomm = new SqlCommandBuilder(_sqlDataAdapter);

                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                }
                else
                {
                    _sqlCommand.Transaction = _sqlTransaction;
                }

                var tableAux = new DataTable();
                _sqlDataAdapter.Fill(tableAux);
                var command = sqlcomm.GetInsertCommand(true);
                if (insertedId)
                {
                    // Specificato database nel nome tabella
                    var findDatabase = table.TableName.Count(c => c == '.');
                    if (findDatabase == 2)
                    {
                        try
                        {
                            var primaryKey = table.PrimaryKey.FirstOrDefault() == null ? "ID" : table.PrimaryKey.FirstOrDefault().ColumnName;
                            command.CommandText =
                                $"{command.CommandText}\n SELECT MAX({primaryKey}) as InsertedId FROM {table.TableName}";
                        }
                        catch (Exception)
                        {
                            command.CommandText = $"{command.CommandText}\n SELECT -1 as InsertedId";
                        }
                    }
                    else
                    {
                        command.CommandText = $"{command.CommandText}\n SELECT SCOPE_IDENTITY() as InsertedId";
                    }
                }

                return command;
            }
            finally
            {
                if (!_transactionActive)
                    _sqlConnection.Close();
            }
        }

        public override DataTable CloneDataTable(string tableName)
        {
            var datatable = new DataTable();
            try
            {
                _sqlCommand.CommandType = CommandType.Text;
                _sqlCommand.CommandText = $"Select top 0 * from {tableName} (nolock) ";
                _sqlCommand.Connection = _sqlConnection;
                _sqlCommand.Parameters.Clear();

                _sqlDataAdapter = new SqlDataAdapter(_sqlCommand);
                var sqlcomm = new SqlCommandBuilder(_sqlDataAdapter);

                if (!_transactionActive)
                {
                    _sqlConnection.ConnectionString = ConnectionString;
                    _sqlConnection.Open();
                }
                else
                {
                    _sqlCommand.Transaction = _sqlTransaction;
                }

                _sqlDataAdapter.Fill(datatable);
            }
            finally
            {
                if (!_transactionActive)
                    _sqlConnection.Close();
            }
            return datatable;
        }

        public override void Dispose()
        {
            if (_sqlConnection != null)
            {
                _sqlConnection.Dispose();
            }
            if (_sqlDataAdapter != null)
            {
                _sqlDataAdapter.Dispose();
            }
            if (_sqlCommand != null)
            {
                _sqlCommand.Dispose();
            }
            if (_sqlTransaction != null)
            {
                _sqlTransaction.Dispose();
            }
        }

        public override bool TestConnection(string connectionString, out string error)
        {
            var ds = ExecuteCommand("select top 0 * from sys.tables", null, false, out error);
            return ds != null && string.IsNullOrEmpty(error);
        }
    }
}
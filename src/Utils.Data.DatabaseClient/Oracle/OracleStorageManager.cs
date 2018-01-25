using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using Utils.Data.DatabaseClient.Abstractions;
using Utils.Extensions.Collections;


namespace Utils.Data.DatabaseClient.Oracle
{
    class OracleStorageManager : StorageManager
    {
        private OracleCommand _oracleCommand;
        private readonly OracleConnection _oracleConnection;
        private OracleTransaction _oracleTransaction;
        private bool _transactionActive;
        private OracleDataAdapter _oracleDataAdapter;

        public OracleStorageManager(string connectionString)
            : base(connectionString)
        {
            _transactionActive = false;
            IsAllOkTransaction = false;
            _oracleConnection = new OracleConnection(connectionString);
            _oracleCommand = new OracleCommand();
        }

        private OracleType GetOracleDbType(object o)
        {
            if (o is string) return OracleType.VarChar;
            if (o is DateTime) return OracleType.DateTime;
            if (o is Int64) return OracleType.Number;
            if (o is Int32) return OracleType.Int32;
            if (o is Int16) return OracleType.Int16;
            if (o is sbyte) return OracleType.Byte;
            if (o is byte) return OracleType.Int16;
            if (o is decimal) return OracleType.Number;
            if (o is float) return OracleType.Float;
            if (o is double) return OracleType.Double;
            if (o is byte[]) return OracleType.Blob;

            return OracleType.VarChar;
        }

        /// <exception cref="ArgumentException">Il valore fornito nel parametro <paramref name="oracleType" /> è un tipo di dati back-end non valido. </exception>
        public override DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error, out Exception exception)
        {
            DataSet dataSet;
            error = string.Empty;
            exception = null;

           

            try
            {
                dataSet = new DataSet();
                _oracleCommand = new OracleCommand
                {
                    CommandType = isStore ? CommandType.StoredProcedure : CommandType.Text,
                    CommandText = storeProcedure,
                    Connection = _oracleConnection
                };

                _oracleCommand.Parameters.Clear();
                if (parameters != null && parameters.Any())
                {
                    _oracleCommand.Parameters.AddRange(parameters.ToArray());
                }
                //this.AddParameters(sqlParameterList, oracleParameters);
                _oracleDataAdapter = new OracleDataAdapter(_oracleCommand);
                if (!_transactionActive)
                {
                    _oracleConnection.ConnectionString = ConnectionString;
                    _oracleConnection.Open();
                    // Massimizzo il tempo di connessione dell'esecuzione su quella impostata nella stringa di connessione
                    _oracleCommand.CommandTimeout = _oracleConnection.ConnectionTimeout;
                }
                else
                {
                    _oracleCommand.Transaction = _oracleTransaction;
                    // Massimizzo il tempo di connessione dell'esecuzione su quella impostata nella transazione
                    _oracleCommand.CommandTimeout = _oracleTransaction.Connection.ConnectionTimeout;
                }
                _oracleDataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                exception = new Exception(
                    $"Errore nell'esecuzione della stored/query {storeProcedure}, sqlParameterList {(parameters != null ? parameters.Select(prm => $"paramName {prm.ParameterName}  - paramValue {prm.Value}").Stringify() : string.Empty)}, isStore {isStore}",
                  ex);
                dataSet = null;
            }
            finally
            {
                if (!_transactionActive)
                {
                    _oracleConnection.Close();
                }
            }
            return dataSet;
        }

      

        public override void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList, bool isStored, out string error, out Exception exception)
        {
            throw new NotImplementedException();
        }

        public override void Insert<TDataSource>(string insert, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList)
        {
            throw new NotImplementedException();
        }

        public override void OpenTransaction(IsolationLevel? iso)
        {
            if (_transactionActive)
            {
                return;
            }
            if (_oracleConnection.State != ConnectionState.Open)
            {
                _oracleConnection.Open();
            }

            _oracleTransaction = iso.HasValue ? _oracleConnection.BeginTransaction(iso.Value) : _oracleConnection.BeginTransaction();
            _transactionActive = true;
        }



        public override void CloseTransaction(bool isAllOk)
        {
            if (_oracleTransaction != null)
            {
                if (IsAllOkTransaction)
                {
                    _oracleTransaction.Commit();
                }
                else
                {
                    _oracleTransaction.Rollback();
                }
            }

            _transactionActive = false;

            _oracleConnection?.Close();
            IsAllOkTransaction = false;
        }

        public override void Dispose()
        {
            _oracleCommand.Dispose();
            _oracleConnection.Dispose();
            _oracleDataAdapter.Dispose();
            _oracleTransaction.Dispose();
        }

        public override bool TestConnection(string connectionString, out string error)
        {
            var oracleManager = new OracleStorageManager(connectionString);

            error = string.Empty;
            ExecuteCommand(@"SELECT TRUNC (SYSDATE, 'MONTH') 'First day of current month' FROM DUAL;", null, false, out error);

            return string.IsNullOrEmpty(error);
        }

        public override DataTable CloneDataTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public override System.Data.SqlClient.SqlCommand GenerateInsertCommand(DataTable table, bool insertedId = true)
        {
            throw new NotImplementedException();
        }

        public override DataTable ExecuteCommandOnDataTable(DataTable dataTable)
        {
            throw new NotImplementedException();
        }


        public override void Commit()
        {
            IsAllOkTransaction = true;
          
        }

        public override void RollBack()
        {
            IsAllOkTransaction = false;
          
        }
    }
}

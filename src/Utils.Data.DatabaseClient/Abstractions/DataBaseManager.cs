using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Utils.Data.DatabaseClient.Abstractions
{
    /// <summary>
    /// classe di gestione del database
    /// </summary>
    public abstract class StorageManager : IStorageManager
    {
        #region Constructors
        

        /// <summary>
        /// costruttore che accetta stringa di connessione esterna
        /// </summary>
        /// <param name="connectionString"> stringa di connessione per l'accesso al db</param>
        protected StorageManager(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString) || connectionString.Length == 0)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            ConnectionString = connectionString;

        }
        #endregion

        #region Properties

        /// <inheritdoc />
        /// <summary>
        /// Ottiene o imposta se la transazione è andata a buon fine
        /// </summary>
        public bool IsAllOkTransaction
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get;
            set;
        }
        #endregion

        #region Static Methods

        /// <summary>
        /// Effettua un test della connessione data la stringa di connessione
        /// </summary>
        /// <param name="connectionString">Stringa di connesione allo storage</param>
        /// <returns>true se si connette, false altrimenti</returns>
        public static bool TestConnectionByConnString(string connectionString)
        {
            return TestConnectionByConnString(connectionString, out var error);
        }

        /// <summary>
        /// Effettua un test della connessione data la stringa di connessione
        /// </summary>
        /// <param name="connectionString">Stringa di connesione allo storage</param>
        /// <param name="error">errore restituito dalla procedura</param>
        /// <returns>true se si connette, false altrimenti</returns>
        public static bool TestConnectionByConnString(string connectionString, out string error)
        {
            // La connection string deve contenere almeno una proprietà con un =
            if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("="))
            {
                return StorageManagerFactory.CreateDatabaseManager(connectionString).TestConnection(connectionString, out error);
            }

            error = $"connectionString non valida: {connectionString}";
            return false;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <returns>restituisce un dataset</returns>
        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters)
        {
            return ExecuteCommand(storeProcedure, parameters, true);
        }

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <returns>restituisce un dataset</returns>
        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore)
        {
            string error;
            return ExecuteCommand(storeProcedure, parameters, isStore, out error);
        }


        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <param name="error">eventuale errore</param>
        /// <returns>restituisce un dataset</returns>
        /// <exception cref="Exception">Eccezione in caso di errore.</exception>
        public DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error)
        {
            Exception exception;
            var ds = ExecuteCommand(storeProcedure, parameters, isStore, out error, out exception);
            if (exception != null)
            {
                throw exception;
            }
            return ds;
        }

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"></param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <param name="error">eventuale errore</param>
        /// <param name="exception">Eccezione in caso di errore</param>
        /// <returns>Data set con i risultati</returns>
        public abstract DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error, out Exception exception);

        /// <summary>
        /// Effettua una query (no stored) sul database
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="sqlParameterList"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <returns>Data set con i risultati</returns>
        public DataSet Query(string storeProcedure, IEnumerable<IDbDataParameter> sqlParameterList = null, bool isStore = false)
        {
            return ExecuteCommand(storeProcedure, sqlParameterList, isStore);
        }


        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSource">Dati da aggiornare</param>
        /// <param name="sqlParameterList">Parametri sql della query</param>
        public void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList)
        {
            string error;
            Update(update, dataSource, sqlParameterList, out error);
        }


        /// <summary>
        /// esegue un update
        /// </summary>
        /// <param name="update">istruzione di update</param>
        /// <param name="dataSource"> </param>
        /// <param name="sqlParameterList">parametri per l'istruzione di update</param>
        /// <param name="isStored"></param>
        /// <exception cref="Exception">Eccezione in caso di errore.</exception>
        public void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList, bool isStored)
        {
            string error;
            Update(update, dataSource, sqlParameterList, isStored, out error);
        }

        /// <summary>
        /// esegue un update
        /// </summary>
        /// <param name="update">istruzione di update</param>
        /// <param name="dataSource"> </param>
        /// <param name="sqlParameterList">parametri per l'istruzione di update</param>
        /// <param name="error">stringa di errore restituita</param>
        /// <exception cref="Exception">Eccezione in caso di errore.</exception>
        public void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList, out string error)
        {
            Update(update, dataSource, sqlParameterList, true, out error);
        }

        /// <summary>
        /// </summary>
        /// <param name="update"></param>
        /// <param name="dataSource"> </param>
        /// <param name="sqlParameterList"></param>
        /// <param name="isStored"></param>
        /// <param name="error"></param>
        /// <exception cref="Exception"></exception>
        public void Update<TDataSource>(string update, TDataSource dataSource, IEnumerable<IDbDataParameter> sqlParameterList, bool isStored, out string error)
        {
            Update(update, dataSource, sqlParameterList, isStored, out error, out var exception);
            if (exception != null)
            {
                throw exception;
            }
        }


        /// <summary>
        /// esegue un update
        /// </summary>
        /// <param name="update">istruzione di update</param>
        /// <param name="dataSource"> Sorgente dati (Admitted types: DataSet,DataTable,DataRow[]</param>
        /// <param name="sqlParameterList">parametri per l'istruzione di update</param>
        /// <param name="isStored">indica se si utilizza una stored procedure per l'update</param>
        /// <param name="error">stringa di errore restituita</param>
        /// <param name="exception"></param>
        public abstract void Update<TDataSource>(string update, TDataSource dataSource,
            IEnumerable<IDbDataParameter> sqlParameterList, bool isStored, out string error, out Exception exception);

        /// <summary>
        /// esegue un insert
        /// </summary>
        /// <param name="insert">istruzione di insert</param>
        /// <param name="dataSource"> </param>
        /// <param name="sqlParameterList">parametri per l'istruzione di insert</param>
        public abstract void Insert<TDataSource>(string insert, TDataSource dataSource,
            IEnumerable<IDbDataParameter> sqlParameterList);

        /// <summary>
        /// apre una transazione per insert o update multipli gestito dalla classe
        /// </summary>
        public void OpenTransaction()
        {
            OpenTransaction(null);
        }

        /// <summary>
        /// apre una transazione per insert o update multipli gestito dalla classe
        /// </summary>
        /// <param name="iso">livello di isolamento nell'esecuzione della transazione</param>
        public abstract void OpenTransaction(IsolationLevel? iso);

        /// <summary>
        /// chiude la transazione gestita lato codice 
        /// </summary>
        /// <param name="isAllOk">se tutto è andato a buon fine esegue la commit</param>
        public abstract void CloseTransaction(bool isAllOk);

        /// <summary>
        /// nel caso in cui la transazione possa fallire solo per chiamate a db allora utilizzo
        /// la variabile di controllo interna 
        /// </summary>
        public virtual void CloseTransaction()
        {
            CloseTransaction(IsAllOkTransaction);
        }
        #endregion

        /// <summary>
        /// Esegue attività definite dall'applicazione, come rilasciare o reimpostare risorse non gestite.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();

        public bool TestConnection()
        {
            return TestConnectionByConnString(ConnectionString, out var error);
        }

        /// <exception cref="Exception">Condition. </exception>
        public bool TestConnection(string connectionString)
        {
            var result = TestConnectionByConnString(connectionString, out var error);
            if (!result)
            {
                //da specializzare
                throw new Exception(error);
            }
            return true;
        }

        public abstract bool TestConnection(string connectionString, out string error);

        public abstract DataTable CloneDataTable(string tableName);

        public int BulkInsert(DataSet set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            var foreignKeys = new Dictionary<DataRelation, int>();
            var relations = set.Relations.Cast<DataRelation>();
            foreach (DataTable table in set.Tables)
            {
                UpdateForeignKeysInTable(table, foreignKeys);

                foreach (DataRow row in table.Rows)
                {
                    var insertCommand = GenerateInsertCommand(table);
                    var parameters = FillParameters(insertCommand, row);

                    var result = ExecuteCommand(insertCommand.CommandText, parameters, false);
                    var insertedId = Convert.ToInt32(result.Tables[0].Rows[0][0]);
                    row[0] = insertedId;
                    //insertCommand.Parameters.Clear();

                    UpdateInsertedIdInRelations(relations.ToList(), table, foreignKeys, insertedId);
                }
            }
            return Convert.ToInt32(set.Tables[0].Rows[0][0]);
        }

        private static void UpdateInsertedIdInRelations(IEnumerable<DataRelation> relations, DataTable table, IDictionary<DataRelation, int> foreignKeys,
          int insertedId)
        {
            var updaterelations = relations.Where(rel => rel.ParentTable.TableName == table.TableName);
            foreach (var updaterelation in updaterelations)
            {
                if (foreignKeys.ContainsKey(updaterelation))
                    foreignKeys[updaterelation] = insertedId;
                else
                {
                    foreignKeys.Add(updaterelation, insertedId);
                }
            }
        }

        private static void UpdateForeignKeysInTable(DataTable table, Dictionary<DataRelation, int> foreignKeys)
        {
            foreach (var foreignKey in foreignKeys)
            {
                if (!table.ParentRelations.Contains(foreignKey.Key.RelationName)) continue;

                foreach (DataRow row in table.Rows)
                {
                    var first = foreignKey.Key.ChildColumns.FirstOrDefault();
                    row[first] = foreignKey.Value;
                }
            }
        }

        private IEnumerable<SqlParameter> FillParameters(SqlCommand insertCommand, DataRow row)
        {
            var parameters = new List<SqlParameter>();

            foreach (SqlParameter parameter in insertCommand.Parameters)
            {
                var colName = parameter.ParameterName.Replace("@", string.Empty);
                parameters.Add(row.Table.Columns.Contains(colName)
                  ? new SqlParameter(parameter.ParameterName, row[colName])
                  : new SqlParameter(parameter.ParameterName, DBNull.Value));
            }

            return parameters;
        }

        public abstract SqlCommand GenerateInsertCommand(DataTable table, bool insertedId = true);

        public abstract DataTable ExecuteCommandOnDataTable(DataTable dataTable);

        public virtual void Commit()
        {
            IsAllOkTransaction = true;
            //this.CloseTransaction(this.IsAllOkTransaction);
        }

        public abstract void RollBack();

        public void InTransaction(Action block, IsolationLevel? isolationLevel = null)
        {
            try
            {
                OpenTransaction(isolationLevel);
                block();
                Commit();
            }
            catch (Exception)
            {
                RollBack();
                throw;
            }
            finally
            {
                CloseTransaction();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;

namespace Utils.Data.DatabaseClient.Abstractions
{
    /// <summary>
    /// Operazioni su uno storage
    /// </summary>
    public interface IStorageOperations
    {
        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="parameters">Parametri sql della query</param>
        void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="parameters">Parametri sql della query</param>
        /// <param name="isStored">Definisce se il comando <see cref="update"/> è una stored Procedure</param>
        void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored);

        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="parameters">Parametri sql della query</param>
        /// <param name="error">Errore restituito dalla procedura</param>
        void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, out string error);

        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="parameters">Parametri sql della query</param>
        /// <param name="isStored">Definisce se il comando <see cref="update"/> è una stored Procedure</param>
        /// <param name="error">Errore restituito dalla procedura</param>
        void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored, out string error);

        /// <summary>
        /// Esegue l'update si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="update">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="parameters">Parametri sql della query</param>
        /// <param name="isStored">Definisce se il comando <see cref="update"/> è una stored Procedure</param>
        /// <param name="error">Errore restituito dalla procedura</param>
        /// <param name="exception">Eccezione restituita dalla procedura</param>
        void Update<TDataSource>(string update, TDataSource dataSet, IEnumerable<IDbDataParameter> parameters, bool isStored, out string error, out Exception exception);

        /// <summary>
        /// Esegue l'insert si un DataSet, DataTable, DataRow[]
        /// </summary>
        /// <typeparam name="TDataSource">Tipo del datasource</typeparam>
        /// <param name="insert">Query di update</param>
        /// <param name="dataSet">Dati da aggiornare</param>
        /// <param name="sqlParameterList">Parametri sql della query</param>
        void Insert<TDataSource>(string insert, TDataSource dataSet, IEnumerable<IDbDataParameter> sqlParameterList);

        /// <summary>
        /// Duplica la struttura della tabella dato il nome
        /// </summary>
        /// <param name="tableName">Nome della tabella</param>
        /// <returns>Dataset contenente la struttura della tabella senza record</returns>
        DataTable CloneDataTable(string tableName);

        /// <summary>
        /// Esegue un update della dataTable 
        /// </summary>
        /// <param name="dataTable">Datatable con la risultante</param>
        /// <returns>DataTable aggiornata</returns>
        DataTable ExecuteCommandOnDataTable(DataTable dataTable);

        /// <summary>
        /// Effettua il bulk insert di un DataSet nello storage
        /// </summary>
        /// <param name="dsTabelle">data set da aggiornare</param>
        /// <returns>L'id inserito della tabella padre</returns>
        int BulkInsert(DataSet dsTabelle);
    }
}
using System;
using System.Collections.Generic;
using System.Data;

namespace Utils.Data.DatabaseClient.Abstractions
{
    /// <summary>
    /// Operazioni di query o stored su storage
    /// </summary>
    public interface IQueryOperations
    {
        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <returns>restituisce un dataset</returns>
        DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters);

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <returns>restituisce un dataset</returns>
        DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore);

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <param name="error">eventuale errore</param>
        /// <returns>restituisce un dataset</returns>
        DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error);

        /// <summary>
        /// Ottiene un DataSet da stored
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <param name="error">eventuale errore</param>
        /// <param name="exception">Eccezione in caso di errore</param>
        /// <returns>Data set con i risultati</returns>
        DataSet ExecuteCommand(string storeProcedure, IEnumerable<IDbDataParameter> parameters, bool isStore, out string error, out Exception exception);


        /// <summary>
        /// Effettua una query (no stored) sul database
        /// </summary>
        /// <param name="storeProcedure"> store procedure da eseguire</param>
        /// <param name="parameters"> serie di parametri</param>
        /// <param name="isStore"> identifica il parametro storeProcedure come il nome di una store o un comando da eseguire come text</param>
        /// <returns>Data set con i risultati</returns>
        DataSet Query(string storeProcedure, IEnumerable<IDbDataParameter> parameters = null, bool isStore = false);
    }
}
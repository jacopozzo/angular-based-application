namespace Utils.Data.DatabaseClient.Abstractions
{
    /// <summary>
    /// Astrazione per il test della connessione
    /// </summary>
    public interface IConnectionChecker
    {
        /// <summary>
        /// Effettua un test della connessione
        /// </summary>
        /// <returns>True se effettua la connessione, false altrimenti</returns>
        bool TestConnection();

        /// <summary>
        /// Effettua un test della connessione
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>True se effettua la connessione, false altrimenti</returns>
        bool TestConnection(string connectionString);

        /// <summary>
        /// Effettua un test della connessione
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="error">errore restituito in caso di connessione non riuscita </param>
        /// <returns>True se effettua la connessione, false altrimenti</returns>
        bool TestConnection(string connectionString, out string error);
    }
}
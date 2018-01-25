using System;
using System.Data;

namespace Utils.Data.DatabaseClient.Abstractions
{
    /// <summary>
    /// Astrazione della gestione di transazione
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// Ottiene o imposta se la transazione è andata a buon fine
        /// </summary>
        bool IsAllOkTransaction { get; set; }

        /// <summary>
        /// Apre una transazione
        /// </summary>
        void OpenTransaction();

        /// <summary>
        /// Apre una transazione
        /// </summary>
        /// <param name="iso">Livello di isolamento della transazione</param>
        void OpenTransaction(IsolationLevel? iso);

        /// <summary>
        /// Chiude la transazione
        /// </summary>
        /// <param name="isAllOk"></param>
        void CloseTransaction(bool isAllOk);

        /// <summary>
        /// Chiude la transazione
        /// </summary>
        void CloseTransaction();

        /// <summary>
        /// Effettua il commit della transazione
        /// </summary>
        void Commit();

        /// <summary>
        /// Effettua il rollback della transazione
        /// </summary>
        void RollBack();

        /// <summary>
        /// Effettua l'azione in transazione
        /// </summary>
        /// <param name="block">Comandi da eseguire in transazione</param>
        /// <param name="isolationLevel">Livello di isolamento</param>
        void InTransaction(Action block, IsolationLevel? isolationLevel = null);
    }
}
using System;

namespace Cimplicity.Views.Data.Repository
{
    public interface ITransactionManager
    {
        void InTransaction(Action block);

        void OpenTransaction();

        void CloseTransaction();
    }
}
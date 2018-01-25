using System;

namespace Cimplicity.Views.Data.Context
{
    public interface IDbDataContext : IDataContext
    {
        void InTransaction(Action block);
    }
}

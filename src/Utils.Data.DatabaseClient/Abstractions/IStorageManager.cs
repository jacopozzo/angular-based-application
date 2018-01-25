using System;

namespace Utils.Data.DatabaseClient.Abstractions
{
    public interface IStorageManager : IDisposable, ITransactionManager, IConnectionChecker, IStorageOperations,
        IQueryOperations
    {
        string ConnectionString { get; set; }
    }
}

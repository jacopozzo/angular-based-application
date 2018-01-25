using System;
using Utils.Data.DatabaseClient.Abstractions;
using Utils.Data.DatabaseClient.FileSystem;
using Utils.Data.DatabaseClient.Oracle;
using Utils.Data.DatabaseClient.SqlServer;

namespace Utils.Data.DatabaseClient
{
    public class StorageManagerFactory
    {
        public static IStorageManager CreateDatabaseManager(string connectionString, StorageClientType storageClientType = StorageClientType.Default)
        {
            switch (storageClientType)
            {
                case StorageClientType.Default:
                case StorageClientType.SqlServer:
                    return new SqlServerStorageManager(connectionString);
                case StorageClientType.Oracle:
                    return new OracleStorageManager(connectionString);
                case StorageClientType.FileSystem:
                    return new FileSystemStorageManager(connectionString);
                default: throw new InvalidOperationException($"Il tipo di storage {storageClientType} non è supportato");
            }
        }
    }
}
using System;

namespace TskMgr
{
    public enum StorageType
    {
        Json,
        Sqlite
    }

    public static class StorageFactory
    {
        public static ITaskStorage CreateStorage(StorageType storageType, string path = null)
        {
            return storageType switch
            {
                StorageType.Json => new JsonTaskStorage(path),
                StorageType.Sqlite => new SqliteTaskStorage(path),
                _ => throw new ArgumentException($"Неподдерживаемый тип хранилища: {storageType}")
            };
        }
    }
}
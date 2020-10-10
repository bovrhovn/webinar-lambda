﻿namespace Lambada.Generators.Options
{
    public class StorageOptions
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public string UsersTableName { get; set; }
        public string FactoriesTableName { get; set; }
        public string FactoryResultTableName { get; set; }
        public string EmailQueueName { get; set; }
    }
}
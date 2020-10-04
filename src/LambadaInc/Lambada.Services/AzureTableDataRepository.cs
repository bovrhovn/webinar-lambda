using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lambada.Services
{
    public abstract class AzureTableDataRepository<T> where T : ITableEntity, new()
    {
        protected readonly string tableName;

        private readonly CloudStorageAccount account;

        protected AzureTableDataRepository(string connectionString, string tableName)
        {
            this.tableName = tableName;
            account = CloudStorageAccount.Parse(connectionString);
        }

        protected CloudTable TableClient => account.CreateCloudTableClient().GetTableReference(tableName);

        public async Task<T> GetDetailsAsync(string entityId)
        {
            var table = TableClient;

            var retrieveOperation = TableOperation.Retrieve<T>(tableName, entityId);

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result == null)
                throw new KeyNotFoundException($"Link with {entityId} was not found!");

            var model = (T) retrievedResult.Result;

            return model;
        }

        public async Task<T> SingleAsync(string partitionKey, string rowKey)
        {
            var table = TableClient;

            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result == null)
                throw new KeyNotFoundException($"Row with {partitionKey} {rowKey} was not found!");

            var model = (T) retrievedResult.Result;

            return model;
        }

        public async Task<List<T>> FilterEqualAsync(string column, string data)
        {
            var table = TableClient;
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition(
                column,
                QueryComparisons.Equal,
                data));

            TableContinuationToken token = null;
            var list = new List<T>();
            do
            {
                TableQuerySegment<T> resultSegment =
                    await table.ExecuteQuerySegmentedAsync(query,
                        token);
                token = resultSegment.ContinuationToken;

                foreach (T entity in resultSegment.Results)
                {
                    list.Add(entity);
                }
            } while (token != null);

            return list;
        }

        public async Task<List<T>> FilterAsync(string column = "", string query = "")
        {
            var table = TableClient;

            var rangeQuery = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    tableName));

            if (!string.IsNullOrEmpty(column) && string.IsNullOrEmpty(query))
            {
                rangeQuery = new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        column,
                        QueryComparisons.Equal,
                        tableName));
            }

            if (!string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(column))
            {
                rangeQuery = new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition(
                        column,
                        QueryComparisons.GreaterThanOrEqual,
                        query));
            }

            TableContinuationToken token = null;
            var list = new List<T>();
            do
            {
                TableQuerySegment<T> resultSegment =
                    await table.ExecuteQuerySegmentedAsync(rangeQuery, token);
                token = resultSegment.ContinuationToken;

                foreach (T entity in resultSegment.Results)
                {
                    list.Add(entity);
                }
            } while (token != null);

            return list;
        }

        public async Task<List<T>> PartitionAsync(string partitionKey)
        {
            var table = TableClient;
            var query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal,
                partitionKey));

            TableContinuationToken token = null;
            var list = new List<T>();
            do
            {
                TableQuerySegment<T> resultSegment =
                    await table.ExecuteQuerySegmentedAsync(query,
                        token);
                token = resultSegment.ContinuationToken;

                foreach (T entity in resultSegment.Results)
                {
                    list.Add(entity);
                }
            } while (token != null);

            return list;
        }

        public async Task<bool> InsertAsync(T model)
        {
            var table = TableClient;

            var insertOperation = TableOperation.InsertOrReplace(model);

            try
            {
                await table.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(T model)
        {
            var table = TableClient;

            var retrieveOperation = TableOperation.Retrieve<T>(model.PartitionKey, model.RowKey);

            var retrievedResult = await table.ExecuteAsync(retrieveOperation);

            var deleteEntity = (T) retrievedResult.Result;

            if (deleteEntity == null) return false;

            try
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);
                await table.ExecuteAsync(deleteOperation);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateAsync(T model)
        {
            var table = TableClient;

            var updateOperation = TableOperation.Replace(model);

            try
            {
                await table.ExecuteAsync(updateOperation);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
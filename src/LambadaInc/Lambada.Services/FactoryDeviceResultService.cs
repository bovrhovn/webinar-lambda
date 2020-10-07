using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lambada.Services
{
    public class FactoryDeviceResultService : AzureTableDataRepository<FactoryDeviceResultModel>,
        IFactoryResultRepository
    {
        public FactoryDeviceResultService(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        public async Task<List<FactoryDeviceResult>> SearchFactoryAsync(string query)
        {
            var lambadaUserModels = await FilterAsync("Name", query);
            var list = new List<FactoryDeviceResult>();
            lambadaUserModels.ForEach(d => list.Add(d.ToFactoryDeviceResult()));
            return list;
        }

        public async Task<List<FactoryDeviceResult>> SearchHoursAgoAsync(int hours)
        {
            var table = TableClient;

            var currentHours = DateTime.Now.AddHours(-hours);

            var rangeQuery = new TableQuery<FactoryDeviceResultModel>()
                .Where(TableQuery.GenerateFilterCondition(
                    "DateCreated",
                    QueryComparisons.GreaterThanOrEqual,
                    currentHours.ToString(CultureInfo.InvariantCulture)));

            TableContinuationToken token = null;
            var list = new List<FactoryDeviceResultModel>();
            do
            {
                var resultSegment =
                    await table.ExecuteQuerySegmentedAsync(rangeQuery, token);
                token = resultSegment.ContinuationToken;

                list.AddRange(resultSegment.Results);
                
            } while (token != null);
            var currentList = new List<FactoryDeviceResult>();
            currentList.ForEach(d => list.Add(d.ToFactoryDeviceResultModel()));
            return currentList;
        }

        public async Task<List<FactoryDeviceResult>> GetAllAsync()
        {
            var lambadaUserModels = await FilterAsync();
            var list = new List<FactoryDeviceResult>();
            lambadaUserModels.ForEach(d => list.Add(d.ToFactoryDeviceResult()));
            return list;
        }

        public async Task<bool> AddAsync(FactoryDeviceResult factoryDeviceResult)
        {
            factoryDeviceResult.DateCreated = DateTime.Now;
            var lambadaUserModel = factoryDeviceResult.ToFactoryDeviceResultModel();
            lambadaUserModel.PartitionKey = tableName;

            return await InsertAsync(lambadaUserModel);
        }

        public async Task<bool> UpdateAsync(FactoryDeviceResult factoryDeviceResult)
        {
            var data = await SingleAsync(tableName, factoryDeviceResult.FactoryDeviceResultId);

            data.FactoryDeviceId = factoryDeviceResult.FactoryDeviceId;
            data.Quantity = factoryDeviceResult.Quantity;

            try
            {
                await UpdateAsync(data);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                return false;
            }

            return true;
        }

        public async Task<FactoryDeviceResult> GetDataAsync(string factoryDataResultId)
        {
            var data = await SingleAsync(tableName, factoryDataResultId);
            return data.ToFactoryDeviceResult();
        }

        public Task<bool> DeleteAsync(string factoryDeviceResultId) => base.DeleteAsync(new FactoryDeviceResultModel
        {
            FactoryDeviceResultId = factoryDeviceResultId, PartitionKey = tableName
        });
    }
}
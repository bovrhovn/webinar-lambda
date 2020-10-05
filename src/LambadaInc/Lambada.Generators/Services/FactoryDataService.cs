using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Generators.Helpers;
using Lambada.Generators.ViewModels;
using Lambada.Interfaces;
using Lambada.Models;
using Lambada.Services;

namespace Lambada.Generators.Services
{
    public class FactoryDataService : AzureTableDataRepository<FactoryModel>, IFactoryRepository
    {
        public FactoryDataService(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        public async Task<List<Factory>> SearchFactoryAsync(string query)
        {
            var lambadaUserModels = await FilterAsync("Name", query);
            var list = new List<Factory>();
            lambadaUserModels.ForEach(d => list.Add(d.ToFactory()));
            return list;
        }

        public async Task<bool> AddAsync(Factory factory)
        {
            factory.DateCreated = DateTime.Now;
            factory.DeviceCount = 0;
            factory.ItemsProduced = 0;
            var lambadaUserModel = factory.ToFactoryModel();
            lambadaUserModel.PartitionKey = tableName;

            return await InsertAsync(lambadaUserModel);
        }

        public async Task<Factory> GetDataAsync(string factoryId)
        {
            var data = await SingleAsync(tableName, factoryId);
            return data.ToFactory();
        }

        public Task<bool> DeleteAsync(string factoryId) => base.DeleteAsync(new FactoryModel
            {FactoryId = factoryId, PartitionKey = tableName});
    }
}
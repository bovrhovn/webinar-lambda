using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace Lambada.Services
{
    public class FactoryDataServiceCosmoDb : IFactoryRepository
    {
        private readonly string deviceConnectionString;
        private readonly IFactorySearchService factorySearchService;
        private readonly Container container;

        public FactoryDataServiceCosmoDb(string connectionString, string databaseName, string containerName,
            string deviceConnectionString, IFactorySearchService factorySearchService)
        {
            this.deviceConnectionString = deviceConnectionString;
            this.factorySearchService = factorySearchService;
            var cosmosClient = new CosmosClient(connectionString);
            var database = cosmosClient.GetDatabase(databaseName);
            container = database.GetContainer(containerName);
        }

        public Task<List<Factory>> SearchFactoryAsync(string query) =>
            factorySearchService.SearchFactoryAsync(query);

        public async Task<List<Factory>> GetAllAsync()
        {
            var query = "SELECT * FROM factories";

            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = container.GetItemQueryIterator<Factory>(queryDefinition);

            var list = new List<Factory>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                list.AddRange(currentResultSet);
            }

            return list;
        }

        public async Task<bool> AddAsync(Factory factory)
        {
            var response = await container.CreateItemAsync(factory, new PartitionKey(factory.FactoryId));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> UpdateAsync(Factory factory)
        {
            var factoryId = factory.FactoryId;
            var response = await container.ReplaceItemAsync(
                partitionKey: new PartitionKey(factoryId),
                id: factoryId,
                item: new
                {
                    id = factoryId,
                    FactoryId = factory.FactoryId,
                    Description = factory.Description,
                    Name = factory.Name,
                    Latitude = factory.Latitude,
                    Longitude = factory.Longitude,
                    DeviceCount = factory.DeviceCount,
                    ItemsProduced = factory.ItemsProduced,
                    DateCreated = factory.DateCreated,
                });
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<Factory> GetDataAsync(string factoryId)
        {
            var response = await container.ReadItemAsync<Factory>(
                partitionKey: new PartitionKey(factoryId),
                id: factoryId);
            return response.Resource;
        }

        public async Task<bool> DeleteAsync(string factoryId)
        {
            var response = await container.DeleteItemAsync<Factory>(factoryId, new PartitionKey(factoryId));
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<List<FactoryDevice>> GetDevicesAsync(string factoryId)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(deviceConnectionString);
            var query = registryManager.CreateQuery(
                $"SELECT * FROM devices WHERE tags.factory.id = '{factoryId}'", 100);
            var list = new List<FactoryDevice>();
            while (query.HasMoreResults)
            {
                var twins = await query.GetNextAsTwinAsync();
                foreach (var twin in twins)
                {
                    var factory = JsonConvert.DeserializeObject<FactoryData>(twin.Tags.ToJson());
                    list.Add(new FactoryDevice
                    {
                        FactoryId = factory.FactoryId,
                        Model = factory.Model,
                        FactoryDeviceId = twin.DeviceId
                    });
                }
            }

            return list;
        }
    }
}
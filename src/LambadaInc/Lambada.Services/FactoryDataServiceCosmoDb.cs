using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace Lambada.Services
{
    public class CosmoFactory
    {
        [JsonProperty("id")] 
        public string Id { get; set; }
        public string FactoryId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DeviceCount { get; set; }
        public int ItemsProduced { get; set; }
        public string DateCreated { get; set; }
    }

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
            var queryResultSetIterator = container.GetItemQueryIterator<CosmoFactory>(queryDefinition);

            var list = new List<Factory>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentList = await queryResultSetIterator.ReadNextAsync();
                foreach (var cosmo in currentList)
                {
                    list.Add(new Factory
                    {
                        FactoryId = cosmo.FactoryId,
                        Name = cosmo.Name,
                        Description = cosmo.Description,
                        Latitude = cosmo.Latitude.ToString(CultureInfo.InvariantCulture),
                        Longitude = cosmo.Longitude.ToString(CultureInfo.InvariantCulture),
                        DateCreated = DateTime.Parse(cosmo.DateCreated),
                        DeviceCount = cosmo.DeviceCount,
                        ItemsProduced = cosmo.ItemsProduced
                    });
                }
            }

            return list;
        }

        public async Task<bool> AddAsync(Factory factory)
        {
            var factoryId = Guid.NewGuid().ToString();
            try
            {
                var cosmo = new CosmoFactory
                {
                    FactoryId = factoryId,
                    Id = factoryId,
                    Name = factory.Name,
                    Description = factory.Description,
                    Latitude = double.Parse(factory.Latitude),
                    Longitude = double.Parse(factory.Longitude),
                    DateCreated = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    DeviceCount = factory.DeviceCount,
                    ItemsProduced = factory.ItemsProduced
                };

                var response = await container.CreateItemAsync(cosmo,
                    new PartitionKey(factoryId),
                    new ItemRequestOptions {EnableContentResponseOnWrite = false});
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Factory factory)
        {
            var factoryId = factory.FactoryId;
            var response = await container.ReplaceItemAsync(
                partitionKey: new PartitionKey(factoryId),
                id: factoryId,
                item: new CosmoFactory
                {
                    Id = factoryId,
                    FactoryId = factory.FactoryId,
                    Description = factory.Description,
                    Name = factory.Name,
                    Latitude = double.Parse(factory.Latitude),
                    Longitude = double.Parse(factory.Longitude),
                    DeviceCount = factory.DeviceCount,
                    ItemsProduced = factory.ItemsProduced,
                    DateCreated = factory.DateCreated.ToString(CultureInfo.InvariantCulture),
                });
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<Factory> GetDataAsync(string factoryId)
        {
            try
            {
                var response = await container.ReadItemAsync<CosmoFactory>(
                    partitionKey: new PartitionKey(factoryId),
                    id: factoryId);

                var cosmo = response.Resource;
                var factory = new Factory
                {
                    FactoryId = cosmo.FactoryId,
                    Name = cosmo.Name,
                    Description = cosmo.Description,
                    Latitude = cosmo.Latitude.ToString(CultureInfo.InvariantCulture),
                    Longitude = cosmo.Longitude.ToString(CultureInfo.InvariantCulture),
                    DateCreated = DateTime.Parse(cosmo.DateCreated),
                    DeviceCount = cosmo.DeviceCount,
                    ItemsProduced = cosmo.ItemsProduced
                };
                
                return factory;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string factoryId)
        {
            var response = await container.DeleteItemAsync<CosmoFactory>(factoryId, new PartitionKey(factoryId));
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
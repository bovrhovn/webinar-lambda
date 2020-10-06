using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace Lambada.Services
{
    public class FactoryDataService : AzureTableDataRepository<FactoryModel>, IFactoryRepository
    {
        private readonly string deviceConnectionString;

        public FactoryDataService(string connectionString, string tableName, string deviceConnectionString) : base(connectionString, tableName) 
            => this.deviceConnectionString = deviceConnectionString;

        public async Task<List<Factory>> SearchFactoryAsync(string query)
        {
            var lambadaUserModels = await FilterAsync("Name", query);
            var list = new List<Factory>();
            lambadaUserModels.ForEach(d => list.Add(d.ToFactory()));
            return list;
        }

        public async Task<List<Factory>> GetAllAsync()
        {
            var lambadaUserModels = await FilterAsync();
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

        public async Task<bool> UpdateAsync(Factory factory)
        {
            var data = await SingleAsync(tableName, factory.FactoryId);

            data.Name = factory.Name;
            data.Description = factory.Description;
            data.Latitude = factory.Latitude;
            data.Longitude = factory.Longitude;
            data.DeviceCount = factory.DeviceCount;
            data.ItemsProduced = factory.ItemsProduced;

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

        public async Task<Factory> GetDataAsync(string factoryId)
        {
            var data = await SingleAsync(tableName, factoryId);
            return data.ToFactory();
        }

        public Task<bool> DeleteAsync(string factoryId) => base.DeleteAsync(new FactoryModel
        {
            FactoryId = factoryId, PartitionKey = tableName
        });

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
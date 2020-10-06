using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Models;
using Lambada.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public static class HourlyData
    {
        [FunctionName("HourlyData")]
        public static async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, 
            ILogger log)
        {
            log.LogInformation($"Starting data generation at {DateTime.UtcNow}");
            var factories = await GetFactoriesAsync();
            log.LogInformation($"Loaded {factories.Count} factories");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var factory in factories)
            {
                log.LogInformation($"Loading devices for factory {factory.Name}");
                var devices = await GetDevicesAsync(factory.FactoryId);
                log.LogInformation($"Device data started at {DateTime.Now} ");
                AddRandomDataAsync(devices);
                log.LogInformation($"Device data finished at {DateTime.Now} ");
            }
            stopWatch.Stop();
            log.LogInformation($"Data for all of the factories done in {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} seconds)");
        }

        /// <summary>
        /// adding random data to the devices in the factory based on their model and different calculations
        /// </summary>
        /// <param name="devices">devices list, registered for specific factory</param>
        private static void AddRandomDataAsync(List<FactoryDevice> devices)
        {
            throw new NotImplementedException();
        }

        public static async Task<List<FactoryDevice>> GetDevicesAsync(string factoryId)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("DeviceConnectionString"));
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

        public static async Task<List<Factory>> GetFactoriesAsync()
        {
            var factoryDataService = new FactoryDataService(Environment.GetEnvironmentVariable("StorageKey"),
                Environment.GetEnvironmentVariable("TableName"));
            return await factoryDataService.GetAllAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LambadaInc.Generators
{
    public class HourlyData
    {
        private readonly IFactoryResultRepository factoryResultRepository;
        private readonly IFactoryRepository factoryRepository;

        public HourlyData(IFactoryResultRepository factoryResultRepository,
            IFactoryRepository factoryRepository)
        {
            this.factoryResultRepository = factoryResultRepository;
            this.factoryRepository = factoryRepository;
        }

        [FunctionName("HourlyData")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"Starting data generation at {DateTime.UtcNow}");
            var factories = await factoryRepository.GetAllAsync();
            log.LogInformation($"Loaded {factories.Count} factories");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            foreach (var factory in factories)
            {
                log.LogInformation($"Loading devices for factory {factory.Name}");
                var devices = await factoryRepository.GetDevicesAsync(factory.FactoryId);
                log.LogInformation($"Device data started at {DateTime.Now} ");
                await AddRandomDataAsync(devices);
                log.LogInformation($"Device data finished at {DateTime.Now} ");
            }

            stopWatch.Stop();
            log.LogInformation(
                $"Data for all of the factories done in {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} seconds)");
        }

        private async Task AddRandomDataAsync(List<FactoryDevice> devices)
        {
            foreach (var device in devices)
            {
                var factoryDeviceResult = new FactoryDeviceResult
                {
                    FactoryDeviceId = device.FactoryDeviceId,
                    DateCreated = DateTime.Now,
                    Quantity = device.Model switch
                    {
                        "SuperBeerMaker" => 20,
                        "CakeIsGoodBeerIsBetter" => 15,
                        "VertigoBeer" => 10,
                        _ => 30
                    }
                };
                await factoryResultRepository.AddAsync(factoryDeviceResult);
            }
        }
    }
}
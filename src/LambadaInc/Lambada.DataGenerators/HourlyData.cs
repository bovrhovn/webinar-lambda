using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
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
            ILogger log,
            [SignalR(HubName = "messages", ConnectionStringSetting = "AzureSignalRConnectionString")]
            IAsyncCollector<SignalRMessage> signalRMessages)
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
                int counter = 0;
                float moneyExpected = 0;
                const float beerCost = 2.5f;
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

                    var message =
                        $"Beer from {factoryDeviceResult.FactoryDeviceId} has been produces in {factoryDeviceResult.Quantity}.";

                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "broadcastMessage",
                            Arguments = new object[] {message}
                        });
                    counter++;
                    moneyExpected += factoryDeviceResult.Quantity * beerCost;
                }

                log.LogInformation($"Updating factory with new numbers");
                factory.ItemsProduced += counter;
                await factoryRepository.UpdateAsync(factory);
                log.LogInformation($"Factory {factory.Name} current items {factory.ItemsProduced}");
                log.LogInformation($"$We earned {moneyExpected} €");
                log.LogInformation($"Device data finished at {DateTime.Now} ");
            }

            stopWatch.Stop();
            log.LogInformation(
                $"Data for all of the factories done in {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} seconds)");
        }
    }
}
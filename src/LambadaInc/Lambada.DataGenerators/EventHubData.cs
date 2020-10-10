using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public class EventHubData
    {
        private readonly IFactoryRepository factoryRepository;

        public EventHubData(IFactoryRepository factoryRepository)
        {
            this.factoryRepository = factoryRepository;
        }

        [FunctionName("EventHubData")]
        public async Task RunAsync([TimerTrigger("30 * * * * *")] TimerInfo myTimer,
            ILogger log,
            [EventHub("dest", Connection = "EHConnectionString")]IAsyncCollector<string> outputEvents,
            [SignalR(HubName = "messages")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation($"Starting data generation at {DateTime.UtcNow}");
            var factories = await factoryRepository.GetAllAsync();
            log.LogInformation($"Loaded {factories.Count} factories");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var counterString = Environment.GetEnvironmentVariable("DefaultCounter");

            int defaultCounter = 1000;
            if (!string.IsNullOrEmpty(counterString))
                defaultCounter = int.Parse(counterString);
            
            foreach (var factory in factories)
            {
                log.LogInformation($"Loading devices for factory {factory.Name}");
                var devices = await factoryRepository.GetDevicesAsync(factory.FactoryId);
                log.LogInformation($"Device data started at {DateTime.Now} ");

                int counter = 0;
                foreach (var device in devices)
                {
                    for (int currentCounter = 0; currentCounter < defaultCounter; currentCounter++)
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

                        await outputEvents.AddAsync(JsonConvert.SerializeObject(factoryDeviceResult));

                        var message =
                            $"Beer from {factoryDeviceResult.FactoryDeviceId} has been produces in {factoryDeviceResult.Quantity}.";

                        await signalRMessages.AddAsync(
                            new SignalRMessage
                            {
                                Target = "broadcastMessage",
                                Arguments = new[] {message}
                            });
                        counter++;
                    }
                }

                log.LogInformation($"Updating factory with new numbers");
                factory.ItemsProduced += counter;
                await factoryRepository.UpdateAsync(factory);
                log.LogInformation($"Factory {factory.Name} current items {factory.ItemsProduced}");
                log.LogInformation($"Device data finished at {DateTime.Now} ");
            }

            stopWatch.Stop();
            log.LogInformation(
                $"Data for all of the factories done in {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} seconds)");
        }
    }
}
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public class HourlyData
    {
        private readonly IFactoryResultRepository factoryResultRepository;
        private readonly IFactoryRepository factoryRepository;
        private readonly IAlertService alertService;
        private readonly IUserRepository userRepository;

        public HourlyData(IFactoryResultRepository factoryResultRepository,
            IFactoryRepository factoryRepository,
            IAlertService alertService, IUserRepository userRepository)
        {
            this.factoryResultRepository = factoryResultRepository;
            this.factoryRepository = factoryRepository;
            this.alertService = alertService;
            this.userRepository = userRepository;
        }

        [FunctionName("HourlyData")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            ILogger log,
            [CosmosDB(databaseName: "lambadadb", collectionName: "stats",
                ConnectionStringSetting = "GenerateOptions:CosmosDbConnectionString")]
            IAsyncCollector<FactoryStatModel> stats,
            [SignalR(HubName = "messages", ConnectionStringSetting = "AzureSignalRConnectionString")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            [Queue("lambada-emails")] IAsyncCollector<CloudQueueMessage> messages)
        {
            log.LogInformation($"Starting data generation at {DateTime.UtcNow}");
            var factories = await factoryRepository.GetAllAsync();
            log.LogInformation($"Loaded {factories.Count} factories");
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            int mainCounter = 0;
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
                    mainCounter++;
                    moneyExpected += factoryDeviceResult.Quantity * beerCost;
                }

                log.LogInformation($"Updating factory with new numbers");
                factory.ItemsProduced += counter;
                await factoryRepository.UpdateAsync(factory);
                log.LogInformation($"Factory {factory.Name} current items {factory.ItemsProduced}");
                log.LogInformation($"We earned {moneyExpected} €");
                await stats.AddAsync(new FactoryStatModel
                {
                    FactoryId = factory.FactoryId,
                    DateCreated = DateTime.Now,
                    EarnedMoney = moneyExpected
                });

                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "stats",
                        Arguments = new object[] {moneyExpected}
                    });

                log.LogInformation($"Device data finished at {DateTime.Now} ");
            }

            var users = await alertService.GetUsersWithActivatedNotificationsAsync();

            foreach (var currentUserId in users)
            {
                var user = await userRepository.GetUserDataByIdAsync(currentUserId);
                log.LogInformation($"Sending email to {user.Email}");
                var emailModel = new EmailModel
                {
                    From = "bojan@vrhovnik.net",
                    To = user.Email,
                    Content = $"Finished with {mainCounter} beers produced at {factories.Count} factories",
                    Subject = "Factory information report"
                };
                var emailMessage = JsonConvert.SerializeObject(emailModel);
                await messages.AddAsync(new CloudQueueMessage(emailMessage));
                log.LogInformation($"Email sent to {user.Email}");
            }

            stopWatch.Stop();
            log.LogInformation(
                $"Data for all of the factories done in {stopWatch.ElapsedMilliseconds} ms ({stopWatch.Elapsed.Seconds} seconds)");
        }
    }
}
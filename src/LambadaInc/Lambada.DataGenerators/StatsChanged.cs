using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public class StatsChanged
    {
        private readonly IAlertService alertService;
        private readonly IUserRepository userRepository;

        public StatsChanged(IAlertService alertService, IUserRepository userRepository)
        {
            this.alertService = alertService;
            this.userRepository = userRepository;
        }

        [FunctionName("StatsChanged")]
        public async Task RunAsync([CosmosDBTrigger(
                databaseName: "lambadadb",
                collectionName: "stats",
                ConnectionStringSetting = "GenerateOptions:CosmosDbConnectionString",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input,
            [SignalR(HubName = "stats", ConnectionStringSetting = "AzureSignalRConnectionString")]
            IAsyncCollector<SignalRMessage> signalRMessages,
            [Queue("lambada-emails")] IAsyncCollector<CloudQueueMessage> messages,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
                string message = $"There was {input.Count} modified documents";
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "broadcastMessage",
                        Arguments = new object[] {message}
                    });
                var users = await alertService.GetUsersWithActivatedNotificationsAsync();
                foreach (var document in input)
                {
                    message =
                        $"Stats {document.GetPropertyValue<string>("FactoryId")} has been updated with new money.";

                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "broadcastMessage",
                            Arguments = new object[] {message}
                        });

                    var money = document.GetPropertyValue<double>("EarnedMoney");
                    log.LogInformation($"Retrieved {users.Count} users");
                    foreach (var currentUserId in users)
                    {
                        var user = await userRepository.GetUserDataByIdAsync(currentUserId);
                        log.LogInformation($"Sending email to {user.Email}");
                        var emailModel = new EmailModel
                        {
                            From = "bojan@vrhovnik.net",
                            To = user.Email,
                            Content = $"Stats has changed - money added {money}",
                            Subject = "Factory has earned more money"
                        };
                        var emailMessage = JsonConvert.SerializeObject(emailModel);
                        await messages.AddAsync(new CloudQueueMessage(emailMessage));
                        log.LogInformation($"Email sent to {user.Email}");
                    }
                }
            }
        }
    }
}
using System;
using System.Text;
using System.Threading.Tasks;
using Lambada.Models;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public class AwsHubEventHub
    {
        [FunctionName("AwsHubEventHub")]
        public async Task RunAsync([EventHubTrigger("trucks-from-aws", Connection = "EHConnectionString")]
            EventData truckMobileMessage,
            [CosmosDB(databaseName: "lambadadb", collectionName: "stats",
                ConnectionStringSetting = "GenerateOptions:CosmosDbConnectionString")]
            IAsyncCollector<FactoryStatModel> stats,
            [SignalR(HubName = "messages",ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation($"Processing message from truck event hub, enqueued at {truckMobileMessage.SystemProperties.EnqueuedTimeUtc}");
            var message = Encoding.UTF8.GetString(truckMobileMessage.Body);

            var awsMessage = JsonConvert.DeserializeObject<AwsMessage>(message);

            var moneyExpected = awsMessage.BeerCount * 2.5f;
            //currently only one Factory has been aquired
            await stats.AddAsync(new FactoryStatModel
            {
                FactoryId = "8167f58a-1388-4ae2-819d-538562bb404c",
                DateCreated = DateTime.Now,
                EarnedMoney = moneyExpected
            });
            
            string showMessage = $"I received message from AWS truck";
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "broadcastMessage",
                    Arguments = new object[] {showMessage}
                });
        }
    }

    /// <summary>
    /// TODO: Adrian needs to give me model
    /// </summary>
    public class AwsMessage
    {
        public double BeerCount { get; set; }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace LambadaInc.Generators
{
    public class FactoryUpdated
    {
        [FunctionName("FactoryUpdated")]
        public async Task RunAsync([CosmosDBTrigger(
                databaseName: "lambadadb",
                collectionName: "factories",
                ConnectionStringSetting = "GenerateOptions:CosmosDbConnectionString",
                LeaseCollectionName = "leases",
                CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input,
            [SignalR(HubName = "messages",ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation("Started to react");
            
            if (input != null && input.Count > 0)
            {
                log.LogInformation($"Documents modified {input.Count}");
                log.LogInformation($"First document Id {input[0].Id}");
                string message = $"There was {input.Count} modified documents";
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "broadcastMessage",
                        Arguments = new object[] {message}
                    });
                foreach (var document in input)
                {
                    message = $"Factory {document.GetPropertyValue<string>("Name")} has been updated. Check factories for an update."; 
                    await signalRMessages.AddAsync(
                        new SignalRMessage
                        {
                            Target = "broadcastMessage",
                            Arguments = new object[] {message}
                        });
                }
            }
        }
    }
}
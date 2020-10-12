using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace LambadaInc.Generators
{
    public static class TestSend
    {
        [FunctionName("TestSend")]
        public static async Task RunAsync([TimerTrigger("30 * * * * *")] TimerInfo myTimer,
            ILogger log,
            [SignalR(HubName = "messages", ConnectionStringSetting = "AzureSignalRConnectionString")]
            IAsyncCollector<SignalRMessage> signalRMessages)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
            for (int counter = 0; counter < 1000; counter++)
            {
                var message = $"We are sending at the moment {counter} message through signalr";
                await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "broadcastMessage",
                        Arguments = new[] {message}
                    });
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace LambadaInc.Generators
{
    public class AlertActivation
    {
        private readonly IAlertService alertService;
        private readonly IUserRepository userRepository;

        public AlertActivation(IAlertService alertService, IUserRepository userRepository)
        {
            this.alertService = alertService;
            this.userRepository = userRepository;
        }

        [FunctionName("AlertActivation")]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log,
            [Queue("lambada-emails")]IAsyncCollector<CloudQueueMessage> messages)
        {
            log.LogInformation(eventGridEvent.Data.ToString());
            var users = await alertService.GetUsersWithActivatedNotificationsAsync();
            log.LogInformation($"Retrieved {users.Count} users");
            foreach (var currentUserId in users)
            {
                var user = await userRepository.GetUserDataByIdAsync(currentUserId);
                log.LogInformation($"Sending email to {user.Email}");
                var emailModel =new EmailModel
                {
                    From = "system@beer.me",
                    To = user.Email,
                    Content = $"There has been an error. Check {Environment.NewLine}{eventGridEvent.Data}",
                    Subject = "System error message from beer system"
                };
                var message = JsonConvert.SerializeObject(emailModel);
                await messages.AddAsync(new CloudQueueMessage(message));
                log.LogInformation($"Email sent to {user.Email}");
            }
        }
    }
}
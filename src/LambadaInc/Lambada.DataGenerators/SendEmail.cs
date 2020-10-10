using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace LambadaInc.Generators
{
    public class SendEmail
    {
        private readonly IStorageWorker storageWorker;

        public SendEmail(IStorageWorker storageWorker) => this.storageWorker = storageWorker;

        [FunctionName("SendEmail")]
        public async Task RunAsync([QueueTrigger("lambada-emails")]
            string currentEmail, ILogger log,
            [SendGrid(ApiKey = "SendGridApiKey")]IAsyncCollector<SendGridMessage> messageCollector)
        {
            log.LogInformation($"Prepare email");
            var html = await storageWorker.DownloadAsStringAsync("email.html");
            var emailModel = JsonConvert.DeserializeObject<EmailModel>(currentEmail);
            //replace items ##
            var replaced = html.Replace("##CONTENT", emailModel.Content);
            log.LogInformation("Email was replaced - content " + replaced);
            var message = new SendGridMessage();
            message.AddTo(emailModel.To);
            message.AddContent("text/html", replaced);
            message.SetFrom(new EmailAddress(emailModel.From));
            message.SetSubject(emailModel.Subject);
            log.LogInformation("Sending email to client...");
            await messageCollector.AddAsync(message);
        }
    }
}
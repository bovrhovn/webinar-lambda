using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Lambada.Base
{
    public class AzureEmailNotificationService : INotificationService
    {
        private readonly string queueName;
        readonly CloudStorageAccount storageAccount;
        
        public AzureEmailNotificationService(string connectionString, string queueName)
        {
            this.queueName = queueName;
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public async Task<bool> NotifyAsync(INotification notification)
        {
            var queue = storageAccount.CreateCloudQueueClient()
                .GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            var notificationData = JsonConvert.SerializeObject(notification);
            try
            {
                await queue.AddMessageAsync(new CloudQueueMessage(notificationData));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }
}
using System.Net;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Cosmos;

namespace Lambada.Services
{
    public class CosmosDbAlertService : IAlertService
    {
        private readonly Container container;

        public CosmosDbAlertService(string connectionString, string databaseName, string containerName)
        {
            var cosmosClient = new CosmosClient(connectionString);
            var database = cosmosClient.GetDatabase(databaseName);
            container = database.GetContainer(containerName);
        }

        public async Task<bool> EnableOrDisableAlertsAsync(bool alertsOn, string userId)
        {
            var userModel = new UserAlertModel {UserId = userId, On = alertsOn};
            var response = await container.ReplaceItemAsync(partitionKey: new PartitionKey(userId),
                id: userId,
                item: userModel);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> GetInfoAboutAlertsAsync(string userId)
        {
            var response = await container.ReadItemAsync<UserAlertModel>(
                partitionKey: new PartitionKey(userId),
                id: userId);
            return response.Resource.On;
        }
    }
}
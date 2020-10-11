using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
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
            try
            {
                var response = await container.ReplaceItemAsync(partitionKey: new PartitionKey(userId),
                    id: userId,
                    item: new
                    {
                        On = alertsOn,
                        UserId=userId,
                        id=userId
                    });
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> GetInfoAboutAlertsAsync(string userId)
        {
            try
            {
                var response = await container.ReadItemAsync<UserAlertModel>(
                    partitionKey: new PartitionKey(userId),
                    id: userId);
                return response.Resource.On;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<List<string>> GetUsersWithActivatedNotificationsAsync()
        {
            var query = "SELECT * FROM subscriptions";

            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = container.GetItemQueryIterator<UserAlertModel>(queryDefinition);

            var list = new List<string>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentList = await queryResultSetIterator.ReadNextAsync();
                list.AddRange(from cosmo in currentList where cosmo.On select cosmo.UserId);
            }

            return list;
        }
    }
}
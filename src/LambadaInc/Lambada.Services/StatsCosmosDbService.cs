using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Cosmos;

namespace Lambada.Services
{
    public class StatsCosmosDbService : IStatsService
    {
        private readonly Container container;

        public StatsCosmosDbService(string connectionString, string databaseName, string containerName)
        {
            var cosmosClient = new CosmosClient(connectionString);
            var database = cosmosClient.GetDatabase(databaseName);
            container = database.GetContainer(containerName);
        }

        public async Task<List<FactoryStatModel>> GetStatsForFactoryAsync(string factoryId)
        {
            var query = new QueryDefinition(
                    "select * from stats s where s.FactoryId = @FactoryId ")
                .WithParameter("@FactoryId", factoryId);

            var list = new List<FactoryStatModel>();
            using var resultSet = container.GetItemQueryIterator<FactoryStatModel>(
                query,
                requestOptions: new QueryRequestOptions
                {
                    PartitionKey = new PartitionKey(factoryId),
                    MaxItemCount = 1
                });
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync(); 
                list.AddRange(response);
            }

            return list;
        }
    }
}
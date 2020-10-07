using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lambada.Services
{
    public class FactoryAzureSearchResultService : IFactorySearchResultService
    {
        private readonly string factoriesResultIndex;
        private SearchServiceClient serviceClient;

        public FactoryAzureSearchResultService(string name, string key, string factoriesResultIndex)
        {
            this.factoriesResultIndex = factoriesResultIndex;
            serviceClient = new SearchServiceClient(name, new SearchCredentials(key));
        }

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            var documentIndexClient =
                serviceClient.Indexes.GetClient(factoriesResultIndex);

            var searchParameters = new SearchParameters
            {
                OrderBy = new[] {"DateCreated desc"},
                IncludeTotalResultCount = true
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var searchResult = await documentIndexClient.Documents.SearchAsync<Document>(query,
                searchParameters);

            stopWatch.Stop();

            var list = GetSearchModels(searchResult);

            return (list, stopWatch.Elapsed);
        }

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchByHoursAsync(int hoursAgo, int itemsCount = 50)
        {
            var documentIndexClient =
                serviceClient.Indexes.GetClient(factoriesResultIndex);

            var searchParameters = new SearchParameters
            {
                OrderBy = new[] {"DateCreated desc"},
                IncludeTotalResultCount = true,
                SearchMode = SearchMode.Any,
                HighlightFields = new[] {"Name","Description"},
                Filter = "Timestamp ge " + DateTime.UtcNow.AddHours(-hoursAgo),
                Top = itemsCount
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var searchResult = await documentIndexClient.Documents.SearchAsync<Document>("*",
                searchParameters);

            stopWatch.Stop();

            var list = GetSearchModels(searchResult);

            return (list, stopWatch.Elapsed);
        }

        private static List<SearchModel> GetSearchModels(DocumentSearchResult<Document> searchResult)
        {
            var list = new List<SearchModel>();
            foreach (var currentDocumentSearchResult in searchResult.Results)
            {
                var document = currentDocumentSearchResult.Document;
                var currentDocumentResult = new SearchModel
                {
                    Title = document["Name"].ToString(),
                    Description = document["Description"].ToString(),
                    Route = "/Factories/RawData"
                };
                list.Add(currentDocumentResult);
            }
            return list;
        }
    }
}
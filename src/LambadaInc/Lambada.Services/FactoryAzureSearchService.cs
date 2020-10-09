using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lambada.Services
{
    public class FactoryAzureSearchService : IFactorySearchService
    {
        private readonly string factoriesIndex;
        private readonly SearchServiceClient serviceClient;

        public FactoryAzureSearchService(string name, string key, string factoriesIndex)
        {
            this.factoriesIndex = factoriesIndex;
            serviceClient = new SearchServiceClient(name, new SearchCredentials(key));
        }

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            var (documentSearchResult, estimated) = await SearchAzureAsync(query);
            var list = new List<SearchModel>();
            foreach (var currentDocumentSearchResult in documentSearchResult.Results)
            {
                var document = currentDocumentSearchResult.Document;
                var currentDocumentResult = new SearchModel
                {
                    Title = document.Name,
                    Description = document.Description,
                    Route = "/Factories/Details/" + document.FactoryId
                };
                list.Add(currentDocumentResult);
            }

            return (list, estimated);
        }

        private async Task<(DocumentSearchResult<Factory> Document, TimeSpan Estimated)> SearchAzureAsync(string query)
        {
            var documentIndexClient =
                serviceClient.Indexes.GetClient(factoriesIndex);

            var searchParameters = new SearchParameters
            {
                OrderBy = new[] {"DateCreated desc"},
                IncludeTotalResultCount = true,
                HighlightFields = new[] {"Name", "Description"}
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var searchResult = await documentIndexClient.Documents.SearchAsync<Factory>(query,
                searchParameters);
            stopWatch.Stop();
            return (searchResult, stopWatch.Elapsed);
        }

        public async Task<List<Factory>> SearchFactoryAsync(string query)
        {
            var (documentSearchResult, estimated) = await SearchAzureAsync(query);

            return documentSearchResult
                .Results
                .Select(currentDocumentSearchResult => currentDocumentSearchResult.Document)
                .ToList();
        }
    }
}
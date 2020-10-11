using Lambada.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Lambada.Services
{
    namespace Lambada.Services
    {
        public class FactoryAzureSearchResultService : ISearchFactoryResultService
        {
            private readonly string factoriesResultIndex;
            private readonly SearchServiceClient serviceClient;

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

                var searchResult = await documentIndexClient.Documents.SearchAsync<FactoryDeviceResult>(query,
                    searchParameters);

                stopWatch.Stop();

                var list = GetSearchModels(searchResult);

                return (list, stopWatch.Elapsed);
            }

            public async Task<(List<SearchModel> Items, long ItemCount)> SearchByHoursAsync(int hoursAgo)
            {
                var documentIndexClient =
                    serviceClient.Indexes.GetClient(factoriesResultIndex);

                // var dateCompare = DateTime.Now.AddHours(-hoursAgo);
                // var searchParameters = new SearchParameters
                // {
                //     OrderBy = new[] {"DateCreated desc"},
                //     IncludeTotalResultCount = true,
                //     SearchMode = SearchMode.Any,
                //     Filter = $"Timestamp gt {dateCompare.ToUniversalTime()}",
                //     Top = itemsCount
                // };
                var searchParameters = new SearchParameters
                {
                    OrderBy = new[] {"DateCreated desc"},
                    IncludeTotalResultCount = true,
                    SearchMode = SearchMode.Any
                };

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var searchResult = await documentIndexClient.Documents.SearchAsync<FactoryDeviceResult>("*",
                    searchParameters);

                stopWatch.Stop();
                
                Debug.WriteLine(stopWatch.Elapsed);

                var list = GetSearchModels(searchResult);

                return (list, searchResult.Count ?? 0);
            }

            private static List<SearchModel> GetSearchModels(DocumentSearchResult<FactoryDeviceResult> searchResult)
            {
                var list = new List<SearchModel>();
                foreach (var currentDocumentSearchResult in searchResult.Results)
                {
                    var document = currentDocumentSearchResult.Document;
                    var currentDocumentResult = new SearchModel
                    {
                        Title = document.FactoryDeviceId,
                        Description = $"Device has {document.Quantity} items",
                        Route = "/Factories/RawData"
                    };
                    list.Add(currentDocumentResult);
                }

                return list;
            }
        }
    }
}
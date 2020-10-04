using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Generators.Interfaces;
using Lambada.Interfaces;
using Lambada.Models;
using Microsoft.Extensions.Logging;

namespace Lambada.Generators.Services
{
    public class FactorySearchService : IFactorySearchService
    {
        private readonly IFactoryRepository factoryRepository;
        private readonly ILogger<FactorySearchService> logger;

        public FactorySearchService(IFactoryRepository factoryRepository, ILogger<FactorySearchService> logger)
        {
            this.factoryRepository = factoryRepository;
            this.logger = logger;
        }

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            logger.LogInformation($"Starting with search with {query}");
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var factories = await factoryRepository.SearchFactoryAsync(query);
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            logger.LogInformation($"Search was finished in {elapsed.Milliseconds} ms.");
            
            var list = new List<SearchModel>();
            factories.ForEach(d=>list.Add(new SearchModel
            {
                Title = d.Name,
                Description = d.Description,
                ImageUrl = "",
                Route = $"/Factories/Details/{d.FactoryId}"
            }));
            return (list, elapsed);
        }
    }
}
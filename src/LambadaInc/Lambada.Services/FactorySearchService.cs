using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class FactorySearchService : IFactorySearchService
    {
        private readonly IFactoryRepository factoryRepository;
        
        public FactorySearchService(IFactoryRepository factoryRepository) => this.factoryRepository = factoryRepository;

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var factories = await factoryRepository.SearchFactoryAsync(query);
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;

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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class FactorySearchResultService : ISearchFactoryResultService
    {
        private readonly IFactoryResultRepository factoryResultRepository;

        public FactorySearchResultService(IFactoryResultRepository factoryResultRepository) =>
            this.factoryResultRepository = factoryResultRepository;

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var factories = await factoryResultRepository.SearchFactoryAsync(query);
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;

            var list = new List<SearchModel>();
            factories.ForEach(d => list.Add(new SearchModel
            {
                Title = d.FactoryDeviceResultId,
                Description = d.Quantity.ToString(),
                ImageUrl = "",
                Route = "/Factories/RawData"
            }));
            return (list, elapsed);
        }

        public async Task<(List<SearchModel> Items, long ItemCount)> SearchByHoursAsync(int hoursAgo)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var factories = await factoryResultRepository.SearchHoursAgoAsync(hoursAgo);
            stopwatch.Stop();
            Debug.WriteLine($"Search was retrieved for {stopwatch.Elapsed.Milliseconds} ms.");
            var list = new List<SearchModel>();
            factories.ForEach(d => list.Add(new SearchModel
            {
                Title = d.FactoryDeviceResultId,
                Description = d.Quantity.ToString(),
                ImageUrl = "",
                Route = "/Factories/RawData"
            }));
            
            return (list, factories.Count);
        }
    }
}
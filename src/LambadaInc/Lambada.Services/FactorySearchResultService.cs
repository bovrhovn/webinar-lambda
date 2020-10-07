using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class FactorySearchResultService : IFactorySearchResultService
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

        public async Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchByHoursAsync(int hoursAgo,
            int itemsCount = 50)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            var factories = await factoryResultRepository.SearchHoursAgoAsync(hoursAgo);
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
            var currentList = list.GetRange(0, itemsCount);
            return (currentList, elapsed);
        }
    }
}
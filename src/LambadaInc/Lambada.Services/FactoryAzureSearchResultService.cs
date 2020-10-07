using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class FactoryAzureSearchResultService : IFactorySearchResultService
    {
        public Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchByHoursAsync(int hoursAgo, int itemsCount = 50)
        {
            throw new NotImplementedException();
        }
    }
}
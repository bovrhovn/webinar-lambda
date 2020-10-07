using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Interfaces;
using Lambada.Models;

namespace Lambada.Services
{
    public class FactoryAzureSearchService : IFactorySearchService
    {
        public Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
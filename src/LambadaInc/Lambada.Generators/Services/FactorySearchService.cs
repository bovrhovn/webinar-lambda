using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Generators.Interfaces;
using Lambada.Models;

namespace Lambada.Generators.Services
{
    public class FactorySearchService : IFactorySearchService
    {
        public Task<(List<SearchModel> Items, int Estimated)> SearchAsync(string query)
        {
            return Task.Run(() =>
            {
                var list = new List<SearchModel>();
                int Estimated = 0;
                return (list, Estimated);
            });
        }
    }
}
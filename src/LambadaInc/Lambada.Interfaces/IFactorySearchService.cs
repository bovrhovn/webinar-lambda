using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IFactorySearchService : ISearchService
    {
        Task<List<Factory>> SearchFactoryAsync(string query);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface ISearchFactoryResultService : ISearchService
    {
        Task<(List<SearchModel> Items, long ItemCount)> SearchByHoursAsync(int hoursAgo);
    }
}
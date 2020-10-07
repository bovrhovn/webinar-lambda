using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IFactorySearchResultService : ISearchService
    {
        Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchByHoursAsync(int hoursAgo, int itemsCount = 50);
    }
}
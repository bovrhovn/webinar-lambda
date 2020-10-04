using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface ISearchService
    {
        Task<(List<SearchModel> Items, TimeSpan Estimated)> SearchAsync(string query);
    }
}
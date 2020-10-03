using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface ISearchService
    {
        Task<(List<SearchModel> Items, int Estimated)> SearchAsync(string query);
    }
}
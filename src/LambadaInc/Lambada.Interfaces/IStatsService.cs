using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IStatsService
    {
        Task<List<FactoryStatModel>> GetStatsForFactoryAsync(string factoryId);
    }
}
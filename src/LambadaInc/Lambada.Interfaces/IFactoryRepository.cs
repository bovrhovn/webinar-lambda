using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IFactoryRepository
    {
        Task<List<Factory>> SearchFactoryAsync(string query);
        Task<bool> AddAsync(Factory factory);
        Task<Factory> GetDataAsync(string factoryId);
        Task<bool> DeleteAsync(string factoryId);
    }
}
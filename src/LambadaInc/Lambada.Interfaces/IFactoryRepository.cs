using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IFactoryRepository
    {
        Task<List<Factory>> SearchFactoryAsync(string query);
        Task<List<Factory>> GetAllAsync();
        Task<bool> AddAsync(Factory factory);
        Task<bool> UpdateAsync(Factory factory);
        Task<Factory> GetDataAsync(string factoryId);
        Task<bool> DeleteAsync(string factoryId);
        Task<List<FactoryDevice>> GetDevicesAsync(string factoryId);
    }
}
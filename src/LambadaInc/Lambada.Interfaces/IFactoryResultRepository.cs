using System.Collections.Generic;
using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IFactoryResultRepository
    {
        Task<List<FactoryDeviceResult>> SearchFactoryAsync(string query);
        Task<List<FactoryDeviceResult>> SearchHoursAgoAsync(int hours);
        Task<List<FactoryDeviceResult>> GetAllAsync();
        Task<bool> AddAsync(FactoryDeviceResult factoryDeviceResult);
        Task<bool> UpdateAsync(FactoryDeviceResult factoryDeviceResult);
        Task<FactoryDeviceResult> GetDataAsync(string factoryDeviceResultId);
        Task<bool> DeleteAsync(string factoryDeviceResultId);
    }
}
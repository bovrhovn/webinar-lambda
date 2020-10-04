using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lambada.Interfaces
{
    public interface IDataRepository<TEntity> where TEntity : class
    {
        bool Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetPagedAsync(int pageCount = 20);
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        long Insert(TEntity entity);
        bool Update(TEntity entity);
        Task<TEntity> GetDetailsAsync(string id);
    }
}
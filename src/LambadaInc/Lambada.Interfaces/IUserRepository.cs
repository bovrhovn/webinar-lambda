using System.Threading.Tasks;
using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IUserRepository
    {
        Task<LambadaUser> LoginAsync(string email, string password);
        Task<LambadaUser> RegisterAsync(LambadaUser user);
    }
}
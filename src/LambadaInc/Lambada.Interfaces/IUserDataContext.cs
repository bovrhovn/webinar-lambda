using Lambada.Models;

namespace Lambada.Interfaces
{
    public interface IUserDataContext
    {
        LambadaUser GetCurrentUser();
    }
}
using System.Threading.Tasks;

namespace Lambada.Interfaces
{
    public interface INotificationService
    {
        Task<bool> NotifyAsync(INotification notification);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lambada.Interfaces
{
    public interface IAlertService
    {
        Task<bool> EnableOrDisableAlertsAsync(bool alertsOn, string userId);
        Task<bool> GetInfoAboutAlertsAsync(string userId);
        Task<List<string>> GetUsersWithActivatedNotificationsAsync();
    }
}
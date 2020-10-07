using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Lambada.Generators.Hubs
{
    public class AlertHub : Hub
    {
        public Task BroadcastMessage(string message) =>
            Clients.All.SendAsync("alertMessage", message);
    }
}
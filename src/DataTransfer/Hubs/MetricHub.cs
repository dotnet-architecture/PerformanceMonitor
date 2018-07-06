using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DataTransfer.Hubs
{
    public class MetricHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
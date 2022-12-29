using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Core.Engine;
using WebApplication1.Core.Engine.Imp.Default;
using WebApplication1.Core.Model;

namespace Vanya.PublicApi.Services
{
    public class MarketTrackingHub : Hub
    {
        public static async Task SendToGroup(IHubContext<MarketTrackingHub> hub, string method, string groupName, object message)
        {
            await hub.Clients.Group(groupName).SendAsync(method, message);
        }

        public async Task Sub(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task Unsub(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}

using Microsoft.AspNetCore.SignalR;

namespace Vanya.PublicApi.Hubs;

public class MarketTrackingHub : Hub
{
    public async Task Sub(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task Unsub(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}

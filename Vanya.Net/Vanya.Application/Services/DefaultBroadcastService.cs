using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Vanya.Application.Contracts.ServiceContracts;
using Vanya.Application.Extensions;

namespace Vanya.Application.Services;
internal class DefaultBroadcastService : IBroadcastService
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultBroadcastService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IHubContext GetHubContext(string channel)
    {
        var type = WebApplicationExtensions.GetType(channel);
        var hubContextType = typeof(IHubContext<>).MakeGenericType(type);

        return _serviceProvider.GetService(hubContextType) as IHubContext;
    }

    public async Task SendToGroup(string channel, string group, string method, object data)
    {
        await GetHubContext(channel).Clients.Group(group).SendAsync(method, data);
    }

    public async Task SendToChannel(string channel, string method, object data)
    {
        await GetHubContext(channel).Clients.All.SendAsync(method, data);
    }
}

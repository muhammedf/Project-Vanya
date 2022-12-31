using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;

namespace Vanya.Application.Extensions;
public static class WebApplicationExtensions
{
    private static readonly ConcurrentDictionary<string, Type> hubDictionary = new();

    internal static Type GetType(string channel)
    {
        return hubDictionary[channel];
    }

    public static void MapHubF<T>(this WebApplication webApplication, string pattern) where T : Hub
    {
        webApplication.MapHub<T>(pattern);
        hubDictionary[pattern] = typeof(T);
    }
}

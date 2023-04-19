using Lambot.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lambot.Core.Adapter;

public static class ServiceCollectionExtensions
{
    public static void RegisterAdapter<TAdapter>(this IServiceCollection services)
         where TAdapter : class, IAdapter, new()
    {
        var adapter =  new TAdapter();
        if (AdapterCollection.Adapters.TryAdd(adapter.AdapterType, adapter))
        {
            Console.WriteLine($"Loading LambotAdapter [{adapter.AdapterName}]");
            adapter.OnConfigureService(services);
        }
    }
}
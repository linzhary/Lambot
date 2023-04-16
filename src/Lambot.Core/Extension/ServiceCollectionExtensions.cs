using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAdapter<TAdapter>(this IServiceCollection services)
         where TAdapter : class, IAdapter, new()
    {
        var adapter = new TAdapter();
        Console.WriteLine($"Loading LambotAdapter [{adapter.AdapterName}]");
        return adapter.ConfigureService(services);
    }
}
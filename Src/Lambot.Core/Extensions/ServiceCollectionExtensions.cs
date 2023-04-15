using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lambot.Core;

public class LambotHost : ILambHost
{
    public static IHostBuilder CreateDefaultBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((ctx, services) =>
            {
                services.AddSingleton<LambotSocketService>();

                services.AddHostedService<LambotSocketListener>();
                services.AddHostedService<LambotMessageHandler>();

                services.AddScoped<LambotContext>();
            });
    }
}
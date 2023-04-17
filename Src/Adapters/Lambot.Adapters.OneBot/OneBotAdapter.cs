using Lambot.Core;
using Lambot.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Adapters.OneBot;

public class OneBotAdapter : IAdapter
{
    public IServiceCollection ConfigureService(IServiceCollection services)
    {
        services.AddSingleton<Bot>();
        services.AddSingleton<IEventParser, OneBotEventParser>();
        services.AddScoped<IPluginMatcher, OneBotEventMatcher>();
        return services;
    }

    public string AdapterName => "Lambot.Adapters.OneBot";
}
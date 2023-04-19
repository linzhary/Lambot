using Lambot.Core;
using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Adapters.OneBot;

public class OneBotAdapter : IAdapter
{
    public void OnConfigureService(IServiceCollection services)
    {
        services.AddSingleton<IEventParser, OneBotEventParser>();
        services.AddScoped<IPluginMatcher, OneBotEventMatcher>();
        services.AddScoped<Bot>();

        //Onebot WebSocket Middleware
        services.AddScoped<OneBotWebSocketMiddleware>();
    }

    public void OnBuild(WebApplication app)
    {
        app.UseMiddleware<OneBotWebSocketMiddleware>();
    }

    public AdapterType AdapterType => AdapterType.Onebot;
    public string AdapterName => this.GetType().Namespace;
}
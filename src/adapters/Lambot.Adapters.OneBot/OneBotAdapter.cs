using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Adapters.OneBot;

public class OneBotAdapter : IAdapter
{
    public void OnConfigureService(IServiceCollection services)
    {
        services.AddSingleton<OneBotEventParser>();
        services.AddScoped<OneBotEventMatcher>();
        services.AddScoped<OneBotClient>();

        //Onebot WebSocket Middleware
        services.AddScoped<OneBotWebSocketMiddleware>();
    }

    public void OnBuild(WebApplication app)
    {
        app.UseMiddleware<OneBotWebSocketMiddleware>();
    }

    public string AdapterName => this.GetType().Namespace;
}
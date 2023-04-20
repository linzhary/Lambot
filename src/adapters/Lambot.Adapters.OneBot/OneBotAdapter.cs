using Lambot.Core;
using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lambot.Adapters.OneBot;

public class OneBotAdapter : IAdapter
{
    public void OnConfigureService(IServiceCollection services)
    {
        services.AddSingleton<OneBotClientManager>();
        services.AddSingleton<OneBotEventParser>();

        services.AddScoped<IPluginMatcher, OneBotEventMatcher>();
        services.AddScoped<OneBotClient>(provider =>
        {
            OneBotClient client = null;
            var context = provider.GetRequiredService<LambotContext>();
            var clientManager = provider.GetRequiredService<OneBotClientManager>();
            if (context.ClientId > 0)
            {
                client = clientManager.Get(context.ClientId);
            }
            if (client is null)
            {
                var eventParser = provider.GetRequiredService<OneBotEventParser>();
                var pluginCollection = provider.GetRequiredService<IPluginCollection>();
                var logger = provider.GetRequiredService<ILogger<OneBotClient>>();
                client = new OneBotClient(eventParser, pluginCollection, logger, clientManager);
            }
            return client;
        });

        //Onebot WebSocket Middleware
        services.AddScoped<OneBotWebSocketMiddleware>();
    }

    public void OnBuild(WebApplication app)
    {
        app.UseMiddleware<OneBotWebSocketMiddleware>();
    }

    public string AdapterName => this.GetType().Namespace;
}
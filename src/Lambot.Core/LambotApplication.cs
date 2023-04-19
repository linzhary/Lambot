using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core;

public class LambotApplication
{
    /// <summary>
    /// Create a LambotApplicationBuilder
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static LambotApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        //add websocket support
        builder.Services.AddWebSockets(_ => { });
        
        //Lambot WebSocket Request Middleware
        builder.Services.AddScoped<LambotContext>();
        
        //Lambot WebSocket Service
        builder.Services.AddScoped<LambotSocketService>();
        
        //Lambot WebSocket Message Processor
        builder.Services.AddHostedService<LambotMessageProcessor>();
        
        return new LambotApplicationBuilder(builder);
    }
}
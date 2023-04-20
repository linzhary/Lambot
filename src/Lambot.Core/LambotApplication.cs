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

        //register controllers
        builder.Services.AddControllers().AddNewtonsoftJson();

        builder.Services.AddHttpClient();

        //add websocket support
        builder.Services.AddWebSockets(opts =>
        {
            opts.KeepAliveInterval = TimeSpan.FromMilliseconds(3000);
        });

        return new LambotApplicationBuilder(builder);
    }
}
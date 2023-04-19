using Lambot.Core.Adapter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

public class LambotApplicationBuilder
{
    private readonly WebApplicationBuilder _builder;
    public IServiceCollection Services => _builder.Services;
    public ConfigurationManager Configuration => _builder.Configuration;
    public ILoggingBuilder Logging => _builder.Logging;
    public IWebHostEnvironment Environment => _builder.Environment;
    public ConfigureHostBuilder Host => _builder.Host;
    public ConfigureWebHostBuilder WebHost => _builder.WebHost;
    
    internal LambotApplicationBuilder(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public WebApplication Build()
    {
        var app = _builder.Build();
        app.UseRouting();
        app.UseWebSockets();
        app.MapControllers();
        foreach (var adapter in AdapterCollection.Adapters.Values)
        {
            adapter.OnBuild(app);
        }
        return app;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

public class LambotHostBuilder
{
    internal LambotHostBuilder()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        Console.WriteLine($"Loading Configuration [appsettings.json]");

        Services = new ServiceCollection();
        Services.AddSingleton(Configuration);

        Services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            builder.AddSimpleConsole(opts =>
            {
                opts.IncludeScopes = true;
            });
        });
        Console.WriteLine($"Loading LoggerFactory [ConsoleLogger]");
    }

    /// <summary>
    /// Configuration
    /// </summary>
    public IConfiguration Configuration { get; set; }

    /// <summary>
    /// ServiceCollection
    /// </summary>
    public IServiceCollection Services { get; set; }

    public void RegisterAdapter<TAdapter>()
         where TAdapter : class, IAdapter, new()
    {
        var adapter = new TAdapter();
        Console.WriteLine($"Loading LambotAdapter [{adapter.AdapterName}]");
        adapter.ConfigureService(Services);
    }

    public IApplicationBuilder Build()
    {
        return new LambotApplicationBuilder(Services);
    }
}
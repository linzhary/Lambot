using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Lambot.Core;

internal class LambotApplicationBuilder : IApplicationBuilder
{
    private readonly IServiceCollection _services;

    private static readonly CancellationTokenSource _cancellationTokenSource = new();
    public LambotApplicationBuilder(IServiceCollection services)
    {
        _services = services;
        services.AddSingleton<LambotApplication>();
        services.AddScoped<LambotContext>();
    }    
    public void Run(string serverUrl = "127.0.0.1:8080")
    {
        Console.CancelKeyPress += (sender, e) => _cancellationTokenSource.Cancel();
        using var serviceProvider = _services.BuildServiceProvider();
        var application = serviceProvider.GetRequiredService<LambotApplication>();
        application.Start(serverUrl, _cancellationTokenSource.Token);
    }
}


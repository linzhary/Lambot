using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core;

internal class ApplicationBuilder : IApplicationBuilder
{
    private readonly IServiceCollection _services;

    private static readonly CancellationTokenSource _cancellationTokenSource = new();

    public ApplicationBuilder(IServiceCollection services)
    {
        _services = services;
        services.AddSingleton<Application>();
        services.AddScoped<LambotContext>();
    }

    public void Run(string serverUrl = "127.0.0.1:8080")
    {
        Console.CancelKeyPress += (sender, e) => _cancellationTokenSource.Cancel();
        using var serviceProvider = _services.BuildServiceProvider();
        var application = serviceProvider.GetRequiredService<Application>();
        application.Start(serverUrl, _cancellationTokenSource.Token);
    }
}
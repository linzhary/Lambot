using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

internal class LambotMessageHandler : BackgroundService
{
    private readonly LambotSocketService _socketService;
    private readonly IServiceProvider _rootServiceProvider;
    private readonly ILogger<LambotSocketListener> _logger;

    public LambotMessageHandler(
        LambotSocketService socketService,
        ILogger<LambotSocketListener> logger,
        IServiceProvider rootServiceProvider)
    {
        _socketService = socketService;
        _logger = logger;
        _rootServiceProvider = rootServiceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_socketService.ReceivedQueue.TryDequeue(out var message))
            {
                await Task.Delay(100, stoppingToken);
                continue;
            }
            using var scope = _rootServiceProvider.CreateAsyncScope();
            try
            {
                var resolver = scope.ServiceProvider.GetRequiredService<IEventParser>();
                var @event = resolver.Parse(message);
                if (@event is null) continue;
                var pluginCollection = scope.ServiceProvider.GetRequiredService<IPluginCollection>();
                pluginCollection.OnMessageAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process Message Failure: {message}", message);
            }
        }
    }
}
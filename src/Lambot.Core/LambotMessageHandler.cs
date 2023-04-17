using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

internal class LambotMessageHandler : BackgroundService
{
    private readonly LambotSocketService _socketService;
    private readonly ILogger<LambotSocketListener> _logger;
    private readonly IEventParser _eventParser;
    private readonly IPluginCollection _pluginCollection;

    public LambotMessageHandler(
        LambotSocketService socketService,
        ILogger<LambotSocketListener> logger,
        IEventParser eventParser,
        IPluginCollection pluginCollection)
    {
        _socketService = socketService;
        _logger = logger;
        _eventParser = eventParser;
        _pluginCollection = pluginCollection;
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
            try
            {
                var @event = _eventParser.Parse(message);
                if (@event is null) continue;
                await _pluginCollection.OnMessageAsync(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Process Message Failure: {message}", message);
            }
        }
    }
}
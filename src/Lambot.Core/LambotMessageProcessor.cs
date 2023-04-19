using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

internal class LambotMessageProcessor : BackgroundService
{
    private readonly LambotWebSocketManager _webSocketService;
    private readonly ILogger<LambotMessageProcessor> _logger;
    private readonly IEventParser _eventParser;
    private readonly IPluginCollection _pluginCollection;

    public LambotMessageProcessor(
        LambotWebSocketManager webSocketService,
        ILogger<LambotMessageProcessor> logger,
        IEventParser eventParser,
        IPluginCollection pluginCollection)
    {
        _webSocketService = webSocketService;
        _logger = logger;
        _eventParser = eventParser;
        _pluginCollection = pluginCollection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(100, stoppingToken);
            foreach (var sessionId in _webSocketService.SessionIds())
            {
                _webSocketService.TryStartTask(sessionId, ct =>
                    Task.Factory.StartNew(async () =>
                    {
                        while (!ct.IsCancellationRequested)
                        {
                            if (!_webSocketService.Queue(sessionId).TryDequeue(out var message))
                            {
                                await Task.Delay(100, stoppingToken);
                                continue;
                            }

                            try
                            {
                                var @event = _eventParser.Parse(message);
                                if (@event is null) continue;
                                await _pluginCollection.OnMessageAsync(sessionId, @event);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Process Message Failure: {message}", message);
                            }
                        }
                    }, ct)
                );
            }
        }
    }
}
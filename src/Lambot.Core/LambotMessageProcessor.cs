using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

internal class LambotMessageProcessor : BackgroundService
{
    private readonly LambotWebSocketManager _resourceService;
    private readonly ILogger<LambotMessageProcessor> _logger;
    private readonly IEventParser _eventParser;
    private readonly IPluginCollection _pluginCollection;

    public LambotMessageProcessor(
        LambotWebSocketManager resourceService,
        ILogger<LambotMessageProcessor> logger,
        IEventParser eventParser,
        IPluginCollection pluginCollection)
    {
        _resourceService = resourceService;
        _logger = logger;
        _eventParser = eventParser;
        _pluginCollection = pluginCollection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Parallel.ForEachAsync(_resourceService.ReceivedQueueMap, stoppingToken, async (receivedQueueEnrty, ct) =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!receivedQueueEnrty.Value.TryDequeue(out var message))
                    {
                        await Task.Delay(100, stoppingToken);
                        continue;
                    }

                    try
                    {
                        var @event = _eventParser.Parse(message);
                        if (@event is null) continue;
                        await _pluginCollection.OnMessageAsync(receivedQueueEnrty.Key, @event);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Process Message Failure: {message}", message);
                    }
                }
            });
            await Task.Delay(100, stoppingToken);
        }
    }
}
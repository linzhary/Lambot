using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Lambot.Core;

public class LambotSocketService
{
    internal readonly ConcurrentQueue<string> ReceivedQueue = new();
    internal readonly ConcurrentQueue<string> MessageSendQueue = new();
    private ClientWebSocket _cws;

    private readonly ILogger<LambotSocketService> _logger;

    public LambotSocketService(ILogger<LambotSocketService> logger)
    {
        _logger = logger;
    }

    internal async Task<bool> TryConnectAsync(string serverUrl, CancellationToken stoppingToken)
    {
        if (_cws?.State == WebSocketState.Open)
        {
            return true;
        }
        try
        {
            _cws = new();
            _cws.Options.SetRequestHeader("Content-Type", "application/json");
            await _cws.ConnectAsync(new Uri($"ws://{serverUrl}"), stoppingToken);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError("Connect to ws server [{_serverUrl}] error: [{message}]", serverUrl, ex.Message);
            return false;
        }
        _logger.LogInformation("Connect to ws server [{_serverUrl}] success", serverUrl);
        return true;
    }

    internal async Task<WebSocketReceiveResult> TryReceiveAsync(ArraySegment<byte> buffer, CancellationToken stoppingToken)
    {
        try
        {
            return await _cws.ReceiveAsync(buffer, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Receive data from ws server error: {message}", ex.Message);
            return null;
        }
    }

    public Task SendAsync(string message)
    {
        return _cws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
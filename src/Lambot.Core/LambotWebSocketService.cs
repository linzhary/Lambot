using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

public class LambotWebSocketService : IDisposable
{
    private long _id = -1;
    private ArraySegment<byte> _socketBuffer = new(new byte[1024 * 4]);
    private List<byte> _messageBuffer = new();
    private LambotWebSocketManager _webSocketManager;
    private readonly ILogger<LambotWebSocketService> _logger;

    public LambotWebSocketService(LambotWebSocketManager resourceManager, ILogger<LambotWebSocketService> logger)
    {
        _webSocketManager = resourceManager;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(long sessionId, WebSocket webSocket, HttpContext context)
    {
        _id = sessionId;
        _webSocketManager.Register(sessionId, webSocket, context);
        WebSocketReceiveResult result;
        do
        {
            try
            {
                result = await webSocket.ReceiveAsync(_socketBuffer, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                result = null;
            }

            if (result is not null && !result.CloseStatus.HasValue && result.MessageType == WebSocketMessageType.Text)
            {
                _messageBuffer.AddRange(_socketBuffer.Slice(0, result.Count));
                if (result.EndOfMessage)
                {
                    var message = Encoding.UTF8.GetString(_messageBuffer.ToArray());
                    _webSocketManager.Queue(_id).Enqueue(message);
                    _messageBuffer.Clear();
                }
            }
        } while (result is not null && !result.CloseStatus.HasValue);

        return true;
    }

    public void Dispose()
    {
        _webSocketManager.UnRegister(_id);
    }
}
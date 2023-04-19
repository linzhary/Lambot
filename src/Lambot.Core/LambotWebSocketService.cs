using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Lambot.Core;

public class LambotWebSocketService : IDisposable
{
    private long _id;
    private ArraySegment<byte> _socketBuffer = new(new byte[1024 * 4]);
    private List<byte> _messageBuffer = new();
    private LambotWebSocketManager _webSocketManager;

    public LambotWebSocketService(LambotWebSocketManager resourceManager)
    {
        _webSocketManager = resourceManager;
    }

    public async Task HandleAsync(WebSocket webSocket)
    {
        this._id = _webSocketManager.Register(webSocket);
        WebSocketReceiveResult result;
        do
        {
            result = await webSocket.ReceiveAsync(_socketBuffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
            {
                _messageBuffer.AddRange(_socketBuffer.Slice(0, result.Count));
                if (result.EndOfMessage)
                {
                    var message = Encoding.UTF8.GetString(_messageBuffer.ToArray());
                    _webSocketManager.Queue(this._id).Enqueue(message);
                    _messageBuffer.Clear();
                }
            }
        } while (!result.CloseStatus.HasValue);
    }

    public void Dispose()
    {
        _webSocketManager.UnRegister(this._id);
    }
}
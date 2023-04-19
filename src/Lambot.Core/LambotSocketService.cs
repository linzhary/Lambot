using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Lambot.Core;

public class LambotSocketService
{
    internal readonly ConcurrentQueue<string> ReceivedQueue = new();
    internal readonly ConcurrentQueue<string> MessageSendQueue = new();
    private WebSocket _websocket;
    private ArraySegment<byte> _socketBuffer = new (new byte[1024 * 4]);
    private List<byte> _messageBuffer = new ();
    
    public async Task HandleAsync(WebSocket webSocket)
    {
        _websocket = webSocket;
        WebSocketReceiveResult result;
        do
        {
            result = await _websocket.ReceiveAsync(_socketBuffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
            {
                _messageBuffer.AddRange(_socketBuffer.Slice(0, result.Count));
                if (result.EndOfMessage)
                {
                    var message = Encoding.UTF8.GetString(_messageBuffer.ToArray());
                    this.ReceivedQueue.Enqueue(message);
                    _messageBuffer.Clear();
                }
            }
        } while (!result.CloseStatus.HasValue);
    }

    public Task SendAsync(string message)
    {
        return _websocket.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Lambot.Core;

public class LambotWebSocketManager
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _receivedQueueMap = new();
    private readonly ConcurrentDictionary<string, WebSocket> _webSocketMap = new();

    internal void RemoveReceivedQueue(string id)
    {
        _receivedQueueMap.TryRemove(id, out _);
        _webSocketMap.TryRemove(id, out _);
    }
    internal ConcurrentQueue<string> GetOrAddReceivedQueue(string id)
    {
        return _receivedQueueMap.GetOrAdd(id, _ => new());
    }

    internal WebSocket GetWebSocket(string id)
    {
        if (_webSocketMap.TryGetValue(id, out var webSocket))
        {
            return webSocket;
        }
        return null;
    }

    internal void Register(string id, WebSocket webSocket)
    {
        _receivedQueueMap.GetOrAdd(id, _ => new());
        _webSocketMap.GetOrAdd(id, _ => webSocket);
    }

    internal ConcurrentDictionary<string, ConcurrentQueue<string>> ReceivedQueueMap => _receivedQueueMap;
}
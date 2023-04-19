using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Lambot.Core;

public class LambotWebSocketManager
{
    private long current_id;
    private readonly ConcurrentDictionary<long, WebSocket> _webSocketMap = new();
    private readonly ConcurrentDictionary<long, ConcurrentQueue<string>> _receivedQueueMap = new();
    private readonly ConcurrentDictionary<long, Task> _processorTaskMap = new();
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _cancellationTokenSourceMap = new();

    /// <summary>
    /// 注册资源
    /// </summary>
    /// <param name="webSocket"></param>
    /// <returns></returns>
    internal long Register(WebSocket webSocket)
    {
        _receivedQueueMap.GetOrAdd(current_id, _ => new());
        _webSocketMap.GetOrAdd(current_id, _ => webSocket);
        return current_id++;
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="id"></param>
    internal void UnRegister(long id)
    {
        _receivedQueueMap.TryRemove(id, out _);
        _webSocketMap.TryRemove(id, out _);
        if (_cancellationTokenSourceMap.TryRemove(id, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            _processorTaskMap.TryRemove(id, out _);
        }
    }
    
    /// <summary>
    /// 消息队列
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal ConcurrentQueue<string> Queue(long id)
    {
        return _receivedQueueMap.GetOrAdd(id, _ => new());
    }

    /// <summary>
    /// WebSocket实例
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    internal WebSocket Get(long id)
    {
        if (_webSocketMap.TryGetValue(id, out var webSocket))
        {
            return webSocket;
        }
        return null;
    }

    /// <summary>
    /// 已注册的WebSocket ID列表
    /// </summary>
    /// <returns></returns>
    internal ICollection<long> SessionIds() => _webSocketMap.Keys;

    /// <summary>
    /// 启动一个任务
    /// </summary>
    /// <param name="id"></param>
    /// <param name="predicate"></param>
    internal bool TryStartTask(long id, Func<CancellationToken, Task> predicate)
    {
        if (_processorTaskMap.ContainsKey(id)) return false;
        var cancellationTokenSource = _cancellationTokenSourceMap.GetOrAdd(id, _ => new CancellationTokenSource());
        return _processorTaskMap.TryAdd(id, predicate.Invoke(cancellationTokenSource.Token));
    }
}
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

public class LambotWebSocketManager
{
    private long currentSessionId;
    private readonly ConcurrentDictionary<long, WebSocket> _webSocketMap = new();
    private readonly ConcurrentDictionary<long, ConcurrentQueue<string>> _receivedQueueMap = new();
    private readonly ConcurrentDictionary<long, Task> _processorTaskMap = new();
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _cancellationTokenSourceMap = new();
    private readonly ILogger<LambotWebSocketManager> _logger;

    public LambotWebSocketManager(ILogger<LambotWebSocketManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 注册资源
    /// </summary>
    /// <param name="webSocket"></param>
    /// <returns></returns>
    internal long Register(WebSocket webSocket)
    {
        _receivedQueueMap.GetOrAdd(currentSessionId, _ => new());
        _webSocketMap.GetOrAdd(currentSessionId, _ => webSocket);
        _logger.LogInformation("register resource of {id} from [LambotWebSocketManager]", currentSessionId);
        return currentSessionId++;
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="sessionId"></param>
    internal void UnRegister(long sessionId)
    {
        _logger.LogInformation("unRegister resource of {id} from [LambotWebSocketManager]", sessionId);
        _receivedQueueMap.TryRemove(sessionId, out _);
        _webSocketMap.TryRemove(sessionId, out _);
        if (_cancellationTokenSourceMap.TryRemove(sessionId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            _processorTaskMap.TryRemove(sessionId, out _);
        }
    }
    
    /// <summary>
    /// 消息队列
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    internal ConcurrentQueue<string> Queue(long sessionId)
    {
        return _receivedQueueMap.GetOrAdd(sessionId, _ => new());
    }

    /// <summary>
    /// WebSocket实例
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    internal WebSocket Get(long sessionId)
    {
        if (_webSocketMap.TryGetValue(sessionId, out var webSocket))
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
    /// <param name="sessionId"></param>
    /// <param name="predicate"></param>
    internal bool HandleQueue(long sessionId, Func<ConcurrentQueue<string>, Task> predicate)
    {
        if (_processorTaskMap.ContainsKey(sessionId)) return false;
        var cancellationTokenSource =  _cancellationTokenSourceMap.GetOrAdd(sessionId, _ => new CancellationTokenSource());
        return _processorTaskMap.TryAdd(sessionId, Task.Factory.StartNew(async () =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await predicate.Invoke(this.Queue(sessionId));
            }
        }, cancellationTokenSource.Token));
    }
}
using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace Lambot.Core;

public class LambotWebSocketManager
{
    private const int sessionIdBegin = 1000;
    private readonly ConcurrentDictionary<long, bool> _sessionIdMap = new();
    private readonly ConcurrentDictionary<long, WebSocket> _webSocketMap = new();
    private readonly ConcurrentDictionary<long, ConcurrentQueue<string>> _receivedQueueMap = new();
    private readonly ConcurrentDictionary<long, Task> _processorTaskMap = new();
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _cancellationTokenSourceMap = new();
    private readonly ILogger<LambotWebSocketManager> _logger;
    private readonly object _lock = new();

    public LambotWebSocketManager(ILogger<LambotWebSocketManager> logger)
    {
        _logger = logger;
        for (int i = sessionIdBegin; i < sessionIdBegin + 10; i++)
        {
            _sessionIdMap.TryAdd(i, false);
        }
    }
    
    /// <summary>
    /// 尝试分配一个Session
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public bool TryAllocateSession(out long sessionId)
    {
        lock (_lock)
        {
            foreach (var item in _sessionIdMap)
            {
                if (item.Value) continue;
                sessionId = item.Key;
                _sessionIdMap.TryUpdate(sessionId, true, true);
                return true;
            }

            sessionId = -1;
            return false;
        }
    }

    /// <summary>
    /// 注册资源
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    internal void Register(long sessionId, WebSocket webSocket)
    {
        _logger.LogInformation("register resource of {id} from [LambotWebSocketManager]", sessionId);
        if (sessionId < sessionIdBegin) throw new ArgumentOutOfRangeException(nameof(sessionId));
        _receivedQueueMap.GetOrAdd(sessionId, _ => new());
        _webSocketMap.GetOrAdd(sessionId, _ => webSocket);
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="sessionId"></param>
    internal void UnRegister(long sessionId)
    {
        _logger.LogInformation("unRegister resource of {id} from [LambotWebSocketManager]", sessionId);
        if (sessionId < sessionIdBegin) throw new ArgumentOutOfRangeException(nameof(sessionId));
        _receivedQueueMap.TryRemove(sessionId, out _);
        _webSocketMap.TryRemove(sessionId, out _);
        if (_cancellationTokenSourceMap.TryRemove(sessionId, out var cancellationTokenSource))
        {
            cancellationTokenSource.Cancel();
            _processorTaskMap.TryRemove(sessionId, out _);
        }

        lock (_lock)
        {
            _sessionIdMap.TryUpdate(sessionId, false, false);
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
        var cancellationTokenSource =
            _cancellationTokenSourceMap.GetOrAdd(sessionId, _ => new CancellationTokenSource());
        return _processorTaskMap.TryAdd(sessionId, Task.Factory.StartNew(async () =>
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                await predicate.Invoke(this.Queue(sessionId));
            }
        }, cancellationTokenSource.Token));
    }
}
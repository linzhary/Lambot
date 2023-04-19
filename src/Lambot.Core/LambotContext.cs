using System.Net.WebSockets;
using System.Text;
using Lambot.Core.Exceptions;

namespace Lambot.Core;

public class LambotContext
{
    private readonly LambotWebSocketManager _webSocketManager;

    public LambotContext(LambotWebSocketManager webSocketManager)
    {
        _webSocketManager = webSocketManager;
    }

    public string ServiceId { get; internal set; }
    public bool IsBreaked { get; set; }

    public Exception Skip()
    {
        IsBreaked = false;
        return new LambotContextException("skip")
        {
            IsSkip = true
        };
    }

    public Exception Break()
    {
        IsBreaked = true;
        return new LambotContextException("break")
        {
            IsBreak = true
        };
    }

    public Exception Finish(string message, bool? @break = null)
    {
        if (@break.HasValue)
        {
            IsBreaked = @break.Value;
        }
        return new LambotContextException(message)
        {
            IsFinish = true
        };
    }

    public Task SendAsync(string message, CancellationToken? stoppingToken = null)
    {
        return _webSocketManager.GetWebSocket(ServiceId).SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, stoppingToken ?? CancellationToken.None);
    }
}
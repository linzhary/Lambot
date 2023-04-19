using Lambot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lambot.Adapters.OneBot;

public class OneBotWebSocketMiddleware : IMiddleware
{
    private readonly LambotWebSocketService _webSocketService;
    private readonly LambotWebSocketManager _webSocketManager;
    private readonly ILogger<OneBotWebSocketMiddleware> _logger;
    public OneBotWebSocketMiddleware(LambotWebSocketService webSocketService, ILogger<OneBotWebSocketMiddleware> logger, LambotWebSocketManager webSocketManager)
    {
        _webSocketService = webSocketService;
        _logger = logger;
        _webSocketManager = webSocketManager;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.HasValue) await next(context);
        var requestPath = context.Request.Path.Value!;
        if (requestPath.TrimEnd('/') == "/onebot/v11")
        {
            if (context.WebSockets.IsWebSocketRequest && _webSocketManager.TryAllocateSession(out var sessionId))
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _logger.LogInformation("Receive connetion from {ipAddress}:{port}", context.Connection.RemoteIpAddress,
                    context.Connection.RemotePort);
                await _webSocketService.HandleAsync(sessionId,webSocket);
                _logger.LogInformation("Stop connetion from {ipAddress}:{port}", context.Connection.RemoteIpAddress,
                    context.Connection.RemotePort);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await next(context);
        }
    }
}
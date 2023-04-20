using Lambot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lambot.Adapters.OneBot;

public class OneBotWebSocketMiddleware : IMiddleware
{
    private readonly LambotWebSocketService _webSocketService;
    private readonly LambotWebSocketManager _webSocketManager;
    private readonly ILogger<OneBotWebSocketMiddleware> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    public OneBotWebSocketMiddleware(
        LambotWebSocketService webSocketService,
        ILogger<OneBotWebSocketMiddleware> logger,
        LambotWebSocketManager webSocketManager,
        IHttpClientFactory httpClientFactory)
    {
        _webSocketService = webSocketService;
        _logger = logger;
        _webSocketManager = webSocketManager;
        _httpClientFactory = httpClientFactory;
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
                using var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri($"http://{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}");
                var result = await client.GetStringAsync("get_login_info");
                _logger.LogInformation(result);

                _logger.LogInformation("Receive connetion from {ipAddress}:{port}", context.Connection.RemoteIpAddress,
                    context.Connection.RemotePort);

                await _webSocketService.HandleAsync(sessionId, webSocket, context);
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
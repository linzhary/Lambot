using Lambot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lambot.Adapters.OneBot;

public class OneBotWebSocketMiddleware : IMiddleware
{
    private readonly LambotWebSocketService _service;
    private readonly ILogger<OneBotWebSocketMiddleware> _logger;
    public OneBotWebSocketMiddleware(LambotWebSocketService service, ILogger<OneBotWebSocketMiddleware> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.HasValue) await next(context);
        var requestPath = context.Request.Path.Value!;
        if (requestPath.TrimEnd('/') == "/onebot/v11")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _logger.LogInformation("Receive connetion from {ipAddress}:{port}", context.Connection.RemoteIpAddress,
                    context.Connection.RemotePort);
                await _service.HandleAsync(webSocket);
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
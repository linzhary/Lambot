using Lambot.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lambot.Adapters.OneBot;

public class OneBotWebSocketMiddleware : IMiddleware
{
    private readonly OneBotClient _client;
    private readonly ILogger<OneBotWebSocketMiddleware> _logger;
    public OneBotWebSocketMiddleware(ILogger<OneBotWebSocketMiddleware> logger, OneBotClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.HasValue) await next(context);
        var requestPath = context.Request.Path.Value!;
        if (requestPath.TrimEnd('/') == "/onebot/v11")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                _logger.LogInformation("begin ws connetion with {ipAddress}:{port}", context.Connection.RemoteIpAddress, context.Connection.RemotePort);
                
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                
                //初始化session的webSocket
                _client.InitWebSocket(webSocket);
                //启动消息接收任务
                var receiveTask = _client.BeginReceiveTask();
                //初始化账户信息
                await _client.InitUserInfoAsync();
                
                await receiveTask;
                
                _logger.LogInformation("break ws connetion with {ipAddress}:{port}", context.Connection.RemoteIpAddress, context.Connection.RemotePort);
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
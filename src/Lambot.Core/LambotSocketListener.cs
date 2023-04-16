using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Text;

namespace Lambot.Core;

internal class LambotSocketListener : BackgroundService
{
    private readonly string _serverUrl;
    private readonly List<byte> _messageBuffer = new();
    private readonly ArraySegment<byte> _socketBuffer = new(new byte[1024 * 4]);

    private readonly LambotSocketService _socketService;

    public LambotSocketListener(
        IConfiguration configuration,
        LambotSocketService socketService)
    {
        _serverUrl = configuration.GetValue<string>("ServerUrl");
        _socketService = socketService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _socketService.TryConnectAsync(_serverUrl, stoppingToken))
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = await _socketService.TryReceiveAsync(_socketBuffer, stoppingToken);
                    if (result is null) break;
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        _messageBuffer.AddRange(_socketBuffer.Slice(0, result.Count));
                        if (result.EndOfMessage)
                        {
                            var message = Encoding.UTF8.GetString(_messageBuffer.ToArray());
                            _socketService.ReceivedQueue.Enqueue(message);
                            _messageBuffer.Clear();
                        }
                    }
                }
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
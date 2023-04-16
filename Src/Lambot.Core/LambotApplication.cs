using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Lambot.Core
{
    public class LambotApplication
    {
        private ClientWebSocket _cws;
        private string _serverUrl;
        private readonly IServiceProvider _serviceProvider;
        private CancellationToken _cancellationToken;
        private readonly ConcurrentQueue<string> _queue = new();

        public LambotApplication(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private async Task<bool> TryConnectAsync()
        {
            if (_cws?.State == WebSocketState.Open)
            {
                return true;
            }
            try
            {
                _cws = new();
                _cws.Options.SetRequestHeader("Content-Type", "application/json");
                await _cws.ConnectAsync(new Uri($"ws://{_serverUrl}"), _cancellationToken);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Connect to ws server [{_serverUrl}] failure: [{ex.Message}]");
                await Task.Delay(1000, _cancellationToken);
                return false;
            }
            Console.WriteLine($"Connect to ws server [{_serverUrl}] success");
            return true;
        }

        private async Task TryReceiveAsync()
        {
            var buffers = new List<byte>();
            var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
            while (!_cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await _cws.ReceiveAsync(buffer, _cancellationToken);
                }
                catch (Exception)
                {
                    return;
                }
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    buffers.AddRange(buffer.Slice(0, result.Count));
                    if (result.EndOfMessage)
                    {
                        var message = Encoding.UTF8.GetString(buffers.ToArray());
                        _queue.Enqueue(message);
                        buffers.Clear();
                    }
                }
            }
        }

        internal void Start(string serverUrl, CancellationToken cancellationToken)
        {
            _serverUrl = serverUrl;
            _cancellationToken = cancellationToken;
            var receiveTask = async () =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (await TryConnectAsync())
                    {
                        await TryReceiveAsync();
                    }
                }
            };
            var consumerTask = async () =>
            {
                Console.WriteLine("开始监听消息队列");
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (!_queue.TryDequeue(out var message))
                    {
                        await Task.Delay(100);
                        continue;
                    }
                    using var scope = _serviceProvider.CreateAsyncScope();
                    var resolver = scope.ServiceProvider.GetRequiredService<IEventParser>();
                    var @event = resolver.Parse(message);
                    if (@event is null) continue;
                    var pluginCollection = scope.ServiceProvider.GetRequiredService<IPluginCollection>();
                    pluginCollection.OnMessageAsync(@event);
                }
            };
            Task.WaitAll(new Task[] { consumerTask(), receiveTask() }, cancellationToken: cancellationToken);
        }

        public async Task SendAsync(string message)
        {
            await _cws.SendAsync(Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, _cancellationToken);
        }
    }
}
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lambot.Adapters.OneBot;

public class OneBotClient
{
    public long UserId { get; internal set; } = -1;
    public string? Nickname { get; internal set; }
    private bool _receivable;

    private WebSocket? _webSocket;
    private readonly ArraySegment<byte> _socketBuffer = new(new byte[1024 * 4]);
    private readonly List<byte> _messageBuffer = new();
    private readonly ConcurrentQueue<string> _messageQueue = new();
    private readonly ConcurrentDictionary<string, Action<JObject>> _callbackMap = new();
    private readonly PostEventParser _eventParser;
    private readonly IPluginCollection _pluginCollection;
    private readonly ILogger<OneBotClient> _logger;
    private readonly OneBotClientManager _clientManager;

    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    public OneBotClient(
        PostEventParser eventParser,
        IPluginCollection pluginCollection,
        ILogger<OneBotClient> logger,
        OneBotClientManager clientManager)
    {
        _eventParser = eventParser;
        _pluginCollection = pluginCollection;
        _logger = logger;
        _clientManager = clientManager;
    }

    /// <summary>
    /// 初始化Client的WebSocket
    /// </summary>
    /// <param name="webSocket"></param>
    internal void InitWebSocket(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    /// <summary>
    /// 初始化Client的UserInfo
    /// </summary>
    internal async Task InitUserInfoAsync()
    {
        await SendAsync("get_login_info", messageObj =>
        {
            UserId = messageObj["data"]?.Value<long>("user_id") ?? -1;
            Nickname = messageObj["data"]?.Value<string>("nickname");
            _receivable = true;
            _logger.LogInformation("init client [{user_id}] [{nickname}] success", UserId, Nickname);
            _clientManager.Add(this);
        });
    }

    /// <summary>
    /// 尝试注册回调，失败返回null
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private string? TryRegisterCallback(Action<JObject>? callback)
    {
        if (callback is null) return null;
        var echo = Guid.NewGuid().ToString();
        _callbackMap.TryAdd(echo, callback);
        return echo;
    }

    /// <summary>
    /// 尝试Invoke回调，失败返回false
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool TryInvokeCallback(JObject message)
    {
        if (!message.ContainsKey("echo"))
        {
            return false;
        }

        var echo = message.Value<string>("echo")!;
        if (_callbackMap.TryGetValue(echo, out var callback))
        {
            callback.Invoke(message);
            return true;
        }

        return true;
    }

    /// <summary>
    /// 群消息
    /// </summary>
    /// <param name="group_id"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessageAsync(long group_id, Message message)
    {
        await SendAsync("send_group_msg", new
        {
            group_id,
            message = message.ToString(),
            auto_escape = false
        });
    }

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="user_id">对方 QQ 号</param>
    /// <param name="message">要发送的内容</param>
    /// <param name="group_id">主动发起临时会话时的来源群号(可选, 机器人本身必须是管理员/群主)</param>
    /// <returns></returns>
    public async Task SendPrivateMessageAsync(long user_id, Message message, long? group_id = null)
    {
        await SendAsync("send_private_msg", new
        {
            user_id,
            group_id,
            message = message.ToString(),
            auto_escape = false
        });
    }

    /// <summary>
    /// 调用go-cqhttp api
    /// </summary>
    /// <param name="action"></param>
    /// <param name="params"></param>
    /// <param name="callback"></param>
    public async Task SendAsync(string action, Action<JObject>? callback = null)
    {
        var json = JsonConvert.SerializeObject(new
        {
            action,
            echo = TryRegisterCallback(callback)
        }, _serializerSettings);

        await _webSocket!.SendAsync(
            Encoding.UTF8.GetBytes(json),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }

    /// <summary>
    /// 调用go-cqhttp api
    /// </summary>
    /// <param name="action"></param>
    /// <param name="params"></param>
    /// <param name="callback"></param>
    public async Task SendAsync<T>(string action, T @params, Action<JObject>? callback = null)
    {
        var json = JsonConvert.SerializeObject(new
        {
            action,
            @params,
            echo = TryRegisterCallback(callback)
        }, _serializerSettings);

        await _webSocket!.SendAsync(
            Encoding.UTF8.GetBytes(json),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }

    /// <summary>
    /// 启动一个消息接收任务
    /// </summary>
    /// <returns></returns>
    internal Task BeginMessageReceiveTask()
    {
        return Task.Run(async () =>
        {
            WebSocketReceiveResult? result;
            do
            {
                try
                {
                    result = await _webSocket!.ReceiveAsync(_socketBuffer, CancellationToken.None);
                }
                catch (WebSocketException)
                {
                    result = null;
                }

                if (result is not null && !result.CloseStatus.HasValue &&
                    result.MessageType == WebSocketMessageType.Text)
                {
                    _messageBuffer.AddRange(_socketBuffer.Slice(0, result.Count));
                    if (result.EndOfMessage)
                    {
                        var message = Encoding.UTF8.GetString(_messageBuffer.ToArray());
                        this._messageQueue.Enqueue(message);
                        _messageBuffer.Clear();
                    }
                }
            } while (result is not null && !result.CloseStatus.HasValue);
        });
    }

    /// <summary>
    /// 启动一个消息处理任务
    /// </summary>
    /// <returns></returns>
    internal Task BeginMessageProcessTask()
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                if (_messageQueue.TryDequeue(out var message))
                {
                    try
                    {
                        var messageObj = JObject.Parse(message);
                        if (!this.TryInvokeCallback(messageObj) && _receivable)
                        {
                            var @event = _eventParser.Parse(messageObj);
                            if (@event is null) continue;
                            await _pluginCollection.OnReceiveAsync(UserId, @event);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Process Message Failure: {message}", message);
                    }
                }
            }
        });
    }
}
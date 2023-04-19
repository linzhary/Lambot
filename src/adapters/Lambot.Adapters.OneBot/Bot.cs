using Lambot.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Lambot.Adapters.OneBot;

public class Bot
{
    private readonly LambotWebSocketService _webSockerService;
    private readonly HashSet<long> SuperUsers = new();

    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    public bool IsSuperUser(long userId)
    {
        return SuperUsers.Contains(userId);
    }

    public Bot(LambotWebSocketService service, IConfiguration configuration)
    {
        _webSockerService = service;
        configuration.GetSection("SuperUsers")?.GetChildren()?.ForEach(item =>
        {
            SuperUsers.Add(Convert.ToInt64(item.Value));
        });
    }

    /// <summary>
    /// 群消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="group_id"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessageAsync(long group_id, Message message)
    {
        await CallApi("send_group_msg", new
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
        await CallApi("send_private_msg", new
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
    /// <param name="action">方法</param>
    /// <param name="@params">参数</param>
    /// <returns></returns>
    public async Task CallApi(string action, object @params)
    {
        var json = JsonConvert.SerializeObject(new
        {
            action,
            @params
        }, _serializerSettings);
        await _webSockerService.SendAsync(json);
    }
}
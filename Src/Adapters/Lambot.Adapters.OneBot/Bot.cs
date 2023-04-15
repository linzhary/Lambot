using Lambot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class Bot
{
    private readonly LambotApplication _application;
    private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        NullValueHandling = NullValueHandling.Ignore,
    };

    public Bot(LambotApplication application)
    {
        _application = application;
    }

    /// <summary>
    /// 群消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="group_id"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendGroupMessageAsync(int group_id, Message message)
    {
        var json = JsonConvert.SerializeObject(new
        {
            action = "send_group_msg",
            @params = new
            {
                group_id = group_id,
                message = message.ToString(),
                auto_escape = false
            }
        }, _serializerSettings);
        await _application.SendAsync(json);
    }

    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="user_id">对方 QQ 号</param>
    /// <param name="message">要发送的内容</param>
    /// <param name="group_id">主动发起临时会话时的来源群号(可选, 机器人本身必须是管理员/群主)</param>
    /// <returns></returns>
    public async Task SendPrivateMessageAsync(int user_id, Message message,int? group_id = null)
    {
        var json = JsonConvert.SerializeObject(new
        {
            action = "send_private_msg",
            @params = new
            {
                user_id,
                group_id,
                message = message.ToString(),
                auto_escape = false
            }
        }, _serializerSettings);
        await _application.SendAsync(json);
    }
}

using Lambot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Lambot.Adapters.OneBot;

internal class OneBotEventParser : IEventParser
{
    private readonly Bot _bot;
    private readonly ILogger<OneBotEventParser> _logger;

    public OneBotEventParser(ILogger<OneBotEventParser> logger, Bot bot)
    {
        _bot = bot;
        _logger = logger;
    }

    /// <summary>
    /// 事件解析
    /// </summary>
    /// <param name="eventMessage"></param>
    /// <returns></returns>
    public LambotEvent Parse(string eventMessage)
    {
        var eventObj = JObject.Parse(eventMessage);
        var post_type = eventObj.Value<string>("post_type");
        return post_type switch
        {
            PostType.Message => ParseMessageEvent(eventObj),
            _ => null
        };
    }

    /// <summary>
    /// 消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal MessageEvent ParseMessageEvent(JObject eventObj)
    {
        var message_type = eventObj.Value<string>("message_type");
        return message_type switch
        {
            MessageType.Group => ParseGroupMessageEvent(eventObj),
            MessageType.Private => ParsePriverMessageEvent(eventObj),
            _ => null,
        };
    }

    /// <summary>
    /// 群聊消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal GroupMessageEvent ParseGroupMessageEvent(JObject eventObj)
    {
        var message_id = eventObj.Value<long>("message_id");
        var group_id = eventObj.Value<long>("group_id");
        var user_id = eventObj.Value<long>("user_id");
        var sub_type = eventObj.Value<string>("sub_type");
        var raw_message = eventObj.Value<string>("raw_message");
        _logger.LogInformation("来自群 [{groupId}] 成员 [{userId}] 的{sub_type} [{message_id}]: {raw_message}", group_id, user_id, MessageSubType.GetChinese(sub_type), message_id, raw_message);

        return new GroupMessageEvent
        {
            MessageId = message_id,
            GroupId = group_id,
            UserId = user_id,
            Type = MessageType.Group,
            SubType = sub_type,
            Message =  Message.From(raw_message),
            RawMessage = raw_message
        };
    }

    /// <summary>
    /// 私聊消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal PrivateMessageEvent ParsePriverMessageEvent(JObject eventObj)
    {
        var message_id = eventObj.Value<long>("message_id");
        var user_id = eventObj.Value<long>("user_id");
        var sub_type = eventObj.Value<string>("sub_type");
        var raw_message = eventObj.Value<string>("raw_message");
        _logger.LogInformation("来自好友 [{userId}] 的{sub_type} [{message_id}]: {raw_message}", user_id, MessageSubType.GetChinese(sub_type), message_id, raw_message);

        return new PrivateMessageEvent
        {
            MessageId = message_id,
            UserId = user_id,
            Type = MessageType.Private,
            SubType = sub_type,
            Message = Message.From(raw_message),
            RawMessage = raw_message
        };
    }
}
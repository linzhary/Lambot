using Lambot.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Lambot.Adapters.OneBot;

public class PostEventParser
{
    private readonly ILogger<PostEventParser> _logger;
    private readonly IConfiguration _configuration;
    private readonly List<long> _superUsers = new List<long>();

    private readonly JsonSerializer _deserializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Converters = new JsonConverter[] { new StringToEnumJsonConverter() }
    });

    public PostEventParser(ILogger<PostEventParser> logger, IConfiguration configuration)
    {
        _logger = logger;
        configuration.GetSection("SuperUsers")?.GetChildren()?.ForEach(item =>
        {
            _superUsers.Add(Convert.ToInt64(item.Value));
        });
        _configuration = configuration;
    }

    /// <summary>
    /// 事件解析
    /// </summary>
    /// <param name="eventMessage"></param>
    /// <returns></returns>
    public LambotEvent? Parse(JObject eventMessage)
    {
        var post_type = eventMessage.Value<string>("post_type");
        return MessageUtils.ConvertTo<PostEventType>(post_type) switch
        {
            PostEventType.Message => ParseMessageEvent(eventMessage),
            _ => null
        };
    }

    /// <summary>
    /// 消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal BaseMessageEvent? ParseMessageEvent(JObject eventObj)
    {
        var message_type = eventObj.Value<string>("message_type");
        return MessageUtils.ConvertTo<MessageType>(message_type) switch
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
    internal BaseMessageEvent ParseGroupMessageEvent(JObject eventObj)
    {
        var evt = eventObj.ToObject<GroupMessageEvent>(_deserializer)!;
        if (_superUsers.Contains(evt.Sender.UserId))
        {
            evt.Sender.Role = GroupUserRole.SuperAdmin;
        }
        _logger.LogInformation("来自群 [{groupId}] 成员 [{userId}] 的{sub_type} [{message_id}]: {raw_message}",
            evt.GroupId, evt.UserId, MessageUtils.GetChinese(evt.SubType), evt.MessageId, evt.RawMessage);

        return evt.Convert();
    }

    /// <summary>
    /// 私聊消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal BaseMessageEvent ParsePriverMessageEvent(JObject eventObj)
    {
        var evt = (PrivateMessageEvent)eventObj;

        _logger.LogInformation("来自好友 [{userId}] 的{sub_type} [{message_id}]: {raw_message}",
            evt.UserId, MessageUtils.GetChinese(evt.SubType), evt.MessageId, evt.RawMessage);

        return evt.Convert();
    }
}
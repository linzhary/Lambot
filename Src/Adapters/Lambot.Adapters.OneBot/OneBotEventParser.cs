using Lambot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Lambot.Adapters.OneBot;

public class EnumJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return MessageUtils.ConvertValue(reader.Value, objectType);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }
}

internal class OneBotEventParser : IEventParser
{
    private readonly Bot _bot;
    private readonly ILogger<OneBotEventParser> _logger;
    private readonly JsonSerializer _deserializer;

    public OneBotEventParser(ILogger<OneBotEventParser> logger, Bot bot)
    {
        _bot = bot;
        _logger = logger;
        _deserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            Converters = new[] { new EnumJsonConverter() }
        });
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
        return MessageUtils.ConvertTo<PostType>(post_type) switch
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
    internal MessageEvent ParseGroupMessageEvent(JObject eventObj)
    {
        var evt = eventObj.ToObject<GroupMessageEvent>(_deserializer);

        _logger.LogInformation("来自群 [{groupId}] 成员 [{userId}] 的{sub_type} [{message_id}]: {raw_message}",
            evt.GroupId, evt.UserId, MessageUtils.GetChinese(evt.SubType), evt.MessageId, evt.RawMessage);

        return evt.Convert();
    }

    /// <summary>
    /// 私聊消息解析
    /// </summary>
    /// <param name="eventObj"></param>
    /// <returns></returns>
    internal MessageEvent ParsePriverMessageEvent(JObject eventObj)
    {
        var evt = eventObj.ToObject<PrivateMessageEvent>(_deserializer);

        _logger.LogInformation("来自好友 [{userId}] 的{sub_type} [{message_id}]: {raw_message}",
            evt.UserId, MessageUtils.GetChinese(evt.SubType), evt.MessageId, evt.RawMessage);

        return evt.Convert();
    }
}
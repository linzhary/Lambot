using Lambot.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lambot.Adapters.OneBot;

public abstract class BaseMessageEventSender
{
    /// <summary>
    /// 发送者QQ号
    /// </summary>
    public long UserId { get; set; } = default!;
    /// <summary>
    /// 昵称
    /// </summary>
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 性别
    /// </summary>
    public Sex Sex { get; set; } = default!;

    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; } = default!;
}

public abstract class BaseMessageEvent : LambotEvent
{
    /// <summary>
    /// 消息主类型
    /// </summary>
    public MessageType MessageType { get; set; }

    /// <summary>
    /// 消息子类型
    /// </summary>
    public MessageSubType SubType { get; set; }

    /// <summary>
    /// 消息时间戳(秒)
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 收到消息的Bot Id
    /// </summary>
    public long SelfId { get; set; }

    /// <summary>
    /// 类型化消息体
    /// </summary>
    [JsonIgnore]
    public Message? Message { get; set; }

    public long MessageId { get; set; }

    public string RawMessage { get; set; } = null!;

    internal BaseMessageEvent Convert()
    {
        Message = (Message)RawMessage;
        return this;
    }

    public static explicit operator BaseMessageEvent(JObject eventObj)
    {
        return eventObj.ToObject<BaseMessageEvent>(GlobalConfig.JsonDeserializer) ?? throw new NotSupportedException();
    }
}
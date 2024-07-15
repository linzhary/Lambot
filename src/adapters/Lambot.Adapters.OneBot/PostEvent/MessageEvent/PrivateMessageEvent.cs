using Newtonsoft.Json.Linq;

namespace Lambot.Adapters.OneBot;

public class PrivateMessageEventSender : BaseMessageEventSender
{
    /// <summary>
    /// 当私聊类型为群临时会话时, 群号
    /// </summary>
    public int? GroupId { get; set; } = default;
}

public class PrivateMessageEvent : BaseMessageEvent, IPrivateEvent
{
    public long? GroupId { get; set; } 
    /// <summary>
    /// 消息接收人
    /// </summary>
    public long TargetId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public PrivateMessageEventSender Sender { get; set; } = null!;

    public static explicit operator PrivateMessageEvent(JObject eventObj)
    {
        return eventObj.ToObject<PrivateMessageEvent>(GlobalConfig.JsonDeserializer) ?? throw new NotSupportedException();
    }
}
using Lambot.Core;
using Newtonsoft.Json;

namespace Lambot.Adapters.OneBot;

public class MessageEvent : LambotEvent
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
    public Message Message { get; set; }

    internal MessageEvent Convert()
    {
        Message = Message.Parse(RawMessage);
        return this;
    }
}
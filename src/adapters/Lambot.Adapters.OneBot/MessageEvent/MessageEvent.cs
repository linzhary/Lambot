using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public class MessageEvent : LambotEvent
{
    /// <summary>
    /// 消息主类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 消息子类型
    /// </summary>
    public string SubType { get; set; }

    /// <summary>
    /// 消息时间戳(秒)
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public MessageEventSender Sender { get; set; }

    /// <summary>
    /// 类型化消息体
    /// </summary>
    public Message Message { get; set; }
}
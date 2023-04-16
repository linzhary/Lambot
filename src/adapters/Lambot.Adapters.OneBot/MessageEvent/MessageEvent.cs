using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public class MessageEvent : LambotEvent
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public int MessageId { get; set; }

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
    public int Time { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public MessageEventSender Sender { get; set; }
}
namespace Lambot.Adapters.OneBot;

public class PrivateMessageEvent : MessageEvent
{
    public long? GroupId { get; set; } 
    /// <summary>
    /// 消息接收人
    /// </summary>
    public long TargetId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public PrivateMessageEventSender Sender { get; set; }
}
namespace Lambot.Adapters.OneBot;

public class PrivateMessageEvent : MessageEvent
{
    /// <summary>
    /// 消息接收人
    /// </summary>
    public int TargetId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public PrivateMessageEventSender Sender { get; set; }
}
using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public class PrivateMessageEventSender : MessageEventSender
{
    /// <summary>
    /// 当私聊类型为群临时会话时, 群号
    /// </summary>
    public int GroupId { get; set; } = 0;
}

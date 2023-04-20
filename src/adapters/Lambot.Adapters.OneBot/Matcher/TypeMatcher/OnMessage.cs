using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnMessage : TypeMatcher
{
    /// <summary>
    /// 消息类型，默认匹配所有
    /// </summary>
    public MessageType Type { get; set; } = MessageType.Unknow;

    /// <summary>
    /// 消息子类型，默认匹配所有
    /// </summary>
    public MessageSubType SubType { get; set; } = MessageSubType.Unknow;

    public override bool Check(LambotEvent evt)
    {
        var messageEvent = evt as BaseMessageEvent;
        if (messageEvent is null) return false;
        //消息类型过滤
        if (Type is not MessageType.Unknow && messageEvent.MessageType != Type)
        {
            return false;
        }

        //消息子类型过滤
        if (SubType is not MessageSubType.Unknow && messageEvent.SubType != SubType)
        {
            return false;
        }

        return true;
    }
}
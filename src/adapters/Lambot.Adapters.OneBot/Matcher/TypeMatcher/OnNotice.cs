using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnNotice : TypeMatcher
{
    /// <summary>
    /// 通知类型，默认匹配所有
    /// </summary>
    public NoticeType Type { get; set; } = NoticeType.Unknow;

    /// <summary>
    /// 通知子类型，默认匹配所有
    /// </summary>
    public NoticeSubType SubType { get; set; } = NoticeSubType.Unknow;

    public override bool Check(LambotEvent evt)
    {
        var noticeEvent = evt as BaseNoticeEvent;
        if (noticeEvent is null) return false;
        //通知类型过滤
        if (Type is not NoticeType.Unknow && noticeEvent.NoticeType != Type)
        {
            return false;
        }

        //通知子类型过滤
        if (SubType is not NoticeSubType.Unknow && noticeEvent.SubType != SubType)
        {
            return false;
        }

        return true;
    }
}
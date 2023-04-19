using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnPermission : PermMatcher
{
    /// <summary>
    /// 用户ID，默认允许所有
    /// </summary>
    public long[] Users { get; set; } = new long[0];

    /// <summary>
    /// 群聊ID，默认允许所有
    /// </summary>
    public long[] Groups { get; set; } = new long[0];

    /// <summary>
    /// 群聊角色，默认允许所有
    /// </summary>
    public GroupUserRole Role { get; set; } = GroupUserRole.Unknow;

    public override bool Check(LambotEvent evt)
    {
        var messageEvent = evt as MessageEvent;
        if (messageEvent is null) return false;
        if (messageEvent is GroupMessageEvent groupEvent)
        {
            //群号过滤
            if (Groups is not null && !Groups.Contains(groupEvent.GroupId))
            {
                return false;
            }
            //角色过滤
            if (Role is not GroupUserRole.Unknow
                && (Role & groupEvent.Sender.Role) == 0)
            {
                return false;
            }
        }

        if (Users is not null && !Users.Contains(messageEvent.UserId))
        {
            return false;
        }

        return true;
    }
}
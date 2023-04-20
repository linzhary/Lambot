using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnPermission : PermMatcher
{
    /// <summary>
    /// 用户ID，默认允许所有
    /// </summary>
    public long[] Users { get; set; } = Array.Empty<long>();

    /// <summary>
    /// 群聊ID，默认允许所有
    /// </summary>
    public long[] Groups { get; set; } = Array.Empty<long>();

    /// <summary>
    /// 群聊角色，默认允许所有
    /// </summary>
    public GroupUserRole Role { get; set; } = GroupUserRole.Unknow;

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (messageEvent is GroupMessageEvent groupEvent)
        {
            //群号过滤
            if (Groups.Length > 0 && Array.IndexOf(Groups, groupEvent.GroupId) == -1)
            {
                return false;
            }
            //角色过滤
            if (Role is not GroupUserRole.Unknow && (Role & groupEvent.Sender.Role) == 0)
            {
                return false;

            }
        }

        if (Users.Length > 0 && Array.IndexOf(Users, messageEvent.UserId) == -1)
        {
            return false;
        }

        return true;
    }
}
using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public class GroupMessageEventSender : MessageEventSender
{
    /// <summary>
    /// 不知道啥
    /// </summary>
    public string Area { get; set; }
    /// <summary>
    /// 不知道啥
    /// </summary>
    public string Card { get; set; }
    /// <summary>
    /// 不知道啥
    /// </summary>
    public string Level { get; set; }
    /// <summary>
    /// 用户角色
    /// </summary>
    public GroupUserRole Role { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Title { get; set; }
    public int UserId { get; set; }
}

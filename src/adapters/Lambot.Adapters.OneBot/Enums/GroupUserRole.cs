namespace Lambot.Adapters.OneBot;

/// <summary>
/// 群成员角色
/// </summary>
[Flags]
public enum GroupUserRole
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknow = 0,

    /// <summary>
    /// 普通
    /// </summary>
    Normal = 0x01,

    /// <summary>
    /// 管理
    /// </summary>
    Admin = 0x10,

    /// <summary>
    /// 群主
    /// </summary>
    Owner = 0x100,
}
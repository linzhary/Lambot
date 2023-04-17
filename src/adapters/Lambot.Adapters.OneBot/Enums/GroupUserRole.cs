namespace Lambot.Adapters.OneBot;

/// <summary>
/// 群成员角色
/// </summary>
public enum GroupUserRole
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknow = -1,

    /// <summary>
    /// 普通
    /// </summary>
    Normal = 0x00,

    /// <summary>
    /// 管理
    /// </summary>
    Admin = 0x01,

    /// <summary>
    /// 群主
    /// </summary>
    Owner = 0x10,
}
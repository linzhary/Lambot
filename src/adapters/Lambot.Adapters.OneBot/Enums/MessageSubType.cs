namespace Lambot.Adapters.OneBot;

public enum MessageSubType
{
    /// <summary>
    /// 未知
    /// </summary>
    Unknow,

    /// <summary>
    /// 好友消息
    /// </summary>
    Friend,

    /// <summary>
    /// 群聊消息
    /// </summary>
    Normal,

    /// <summary>
    /// 匿名消息
    /// </summary>
    Anonymous,

    /// <summary>
    /// 自己的群聊消息
    /// </summary>
    GroupSelf,

    /// <summary>
    /// 群临时会话
    /// </summary>
    Group,

    /// <summary>
    /// 系统提示
    /// </summary>
    Notice
}
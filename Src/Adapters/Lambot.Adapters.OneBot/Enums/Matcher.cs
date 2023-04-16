namespace Lambot.Adapters.OneBot;

/// <summary>
/// 匹配器枚举
/// </summary>
public static class Matcher
{
    public enum Type
    {
        /// <summary>
        /// 所有消息
        /// </summary>
        OnMessage = 0x10,

        /// <summary>
        /// 群组消息
        /// </summary>
        OnGroupMessage = 0x11,

        /// <summary>
        /// 私聊消息
        /// </summary>
        OnPrivateMessage = 0x12
    }
}
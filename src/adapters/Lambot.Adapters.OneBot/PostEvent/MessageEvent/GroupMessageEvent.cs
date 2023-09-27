using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Lambot.Adapters.OneBot;

public class GroupMessageEventSender : BaseMessageEventSender
{
    /// <summary>
    /// 不知道啥
    /// </summary>
    public string? Area { get; set; }

    /// <summary>
    /// 不知道啥
    /// </summary>
    public string? Card { get; set; }

    /// <summary>
    /// 不知道啥
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 用户角色
    /// </summary>
    public GroupUserRole Role { get; set; }

    public string? Title { get; set; }

    public long UserId { get; set; }
}

/// <summary>
/// 群聊消息
/// </summary>
public class GroupMessageEvent : BaseMessageEvent
{
    /// <summary>
    /// 消息所在群
    /// </summary>
    public long GroupId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public GroupMessageEventSender Sender { get; set; } = null!;

    public static explicit operator GroupMessageEvent(JObject eventObj)
    {
        return eventObj.ToObject<GroupMessageEvent>(GlobalConfig.JsonDeserializer) ?? throw new NotSupportedException();
    }
}
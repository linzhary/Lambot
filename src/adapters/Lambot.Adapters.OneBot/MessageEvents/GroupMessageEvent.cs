﻿namespace Lambot.Adapters.OneBot;

/// <summary>
/// 群聊消息
/// </summary>
public class GroupMessageEvent : MessageEvent
{
    /// <summary>
    /// 消息所在群
    /// </summary>
    public long GroupId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    public GroupMessageEventSender Sender { get; set; }
}
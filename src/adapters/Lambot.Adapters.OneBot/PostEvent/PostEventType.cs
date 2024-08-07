﻿namespace Lambot.Adapters.OneBot;

/// <summary>
/// 上报类型:
/// </summary>
public enum PostEventType
{
    /// <summary>
    /// 消息, 例如, 群聊消息
    /// </summary>
    Message,

    /// <summary>
    /// 消息发送，例如，bot发送在群里的消息
    /// </summary>
    MessageSent,

    /// <summary>
    /// 请求, 例如, 好友申请
    /// </summary>
    Request,

    /// <summary>
    /// 通知, 例如, 群成员增加
    /// </summary>
    Notice,

    /// <summary>
    /// 元事件, 例如, go-cqhttp 心跳包
    /// </summary>
    MetaEvent
}
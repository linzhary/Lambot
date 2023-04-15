using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public static class PostType
{
    /// <summary>
    /// 消息, 例如, 群聊消息
    /// </summary>
    public const string Message = "message";
    /// <summary>
    /// 请求, 例如, 好友申请
    /// </summary>
    public const string Request = "request";
    /// <summary>
    /// 通知, 例如, 群成员增加
    /// </summary>
    public const string Notice = "notice";
    /// <summary>
    /// 元事件, 例如, go-cqhttp 心跳包
    /// </summary>
    public const string MetaEvent = "meta_event";
}

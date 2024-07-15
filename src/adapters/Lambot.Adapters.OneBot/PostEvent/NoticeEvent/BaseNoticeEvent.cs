using Lambot.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public abstract class BaseNoticeEvent : LambotEvent
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public NoticeType NoticeType { get; set; }

    /// <summary>
    /// 通知子类型
    /// </summary>
    public NoticeSubType SubType { get; set; } = NoticeSubType.Unknow;
    /// <summary>
    /// 消息时间戳(秒)
    /// </summary>
    public long Time { get; set; }
    /// <summary>
    /// 消息发送人
    /// </summary>
    public long UserId { get; set; }
    /// <summary>
    /// 收到消息的Bot Id
    /// </summary>
    public long SelfId { get; set; }

    /// <summary>
    /// 原始消息
    /// </summary>
    public JObject RawMessage { get; set; } = null!;
}

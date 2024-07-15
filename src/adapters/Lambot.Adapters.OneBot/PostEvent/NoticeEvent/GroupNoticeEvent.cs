using Lambot.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class GroupNoticeEvent : BaseNoticeEvent, IGroupEvent
{
    /// <summary>
    /// 群号
    /// </summary>
    public long GroupId { get; set; }
}

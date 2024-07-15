using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class PrivateNoticeEvent : BaseNoticeEvent, IPrivateEvent
{
    /// <summary>
    /// 群号
    /// </summary>
    public long? GroupId { get; set; }
}

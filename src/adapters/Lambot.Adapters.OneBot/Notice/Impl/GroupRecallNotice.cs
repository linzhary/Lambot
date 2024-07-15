using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class GroupRecallNotice : BaseNoticeEvent
{
    public long GroupId { get; set; }
    public long OperatorId { get; set; }
    public long MessageId { get; set; }
}

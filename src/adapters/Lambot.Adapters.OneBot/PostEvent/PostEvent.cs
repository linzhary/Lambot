using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public abstract class PostEvent
{
    public long MessageId { get; set; }
    public string? RawMessage { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Core;

public abstract class LambotEvent
{
    public string RawMessage { get; set; }
}

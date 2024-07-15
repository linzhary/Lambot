using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public interface IGroupEvent
{
    long GroupId { get; set; }
}


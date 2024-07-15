using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public interface IPrivateEvent
{
    long UserId { get; set; }
    long? GroupId { get; set; }
}

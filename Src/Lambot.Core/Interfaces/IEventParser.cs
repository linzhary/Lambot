using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Core;

public interface IEventParser
{
    LambotEvent Parse(string postMessage);
}

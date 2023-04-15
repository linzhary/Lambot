using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Core;

public interface IPluginCollection
{
    void OnMessageAsync(LambotEvent evt);
}

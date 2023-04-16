using Lambot.Adapters.OneBot;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Template.Plugins.FastLearning;

[PluginInfo(Name = "快速学习")]
public class Plugin : PluginBase
{
    private readonly List<int> _allowedGroups = new();

    public Plugin(IConfiguration configuration)
    {
        var section = configuration.GetSection("AllowedGroups");
        foreach(var child in section.GetChildren())
        {
            _allowedGroups.Add(Convert.ToInt32(child.Value));
        }
    }

    [OnGroupMessage(Break = true)]
    public void OnGroupMessage(GroupMessageEvent evt)
    {

    }
}

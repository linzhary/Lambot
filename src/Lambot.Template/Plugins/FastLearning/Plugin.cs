using Lambot.Adapters.OneBot;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.FastLearning;

[PluginInfo(Name = "快速学习")]
public class Plugin : PluginBase
{
    private readonly List<long> _allowedGroups = new();
    private readonly Bot _bot;
    public Plugin(IConfiguration configuration, Bot bot)
    {
        var section = configuration.GetSection("AllowedGroups");
        foreach (var child in section.GetChildren())
        {
            _allowedGroups.Add(Convert.ToInt64(child.Value));
        }
        _bot = bot;
    }

    public bool CheckGroup(long group_id)
    {
        return _allowedGroups.Contains(group_id);    
    }

    [OnRegex(@"我问.*你答.*")]
    [OnGroupMessage(Break = true)]
    public async Task OnGroupMessage(GroupMessageEvent evt)
    {
        if (!CheckGroup(evt.GroupId)) return;
        await _bot.SendGroupMessageAsync(evt.GroupId, Message.From("我记住了~"));
    }
}

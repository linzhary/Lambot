using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.FastLearning;

[PluginInfo(Name = "快速学习")]
public class FastLearningPlugin : PluginBase
{
    private readonly HashSet<long> _allowedGroups = new();
    private readonly Bot _bot;

    private const string ME = "我";
    private const string ANY = "有人";

    //private const string TARGET = ".*CQ:at,qq=.*";
    private readonly FastLearningRepository _repository;

    public FastLearningPlugin(IConfiguration configuration, Bot bot, FastLearningRepository repository)
    {
        configuration.GetSection("AllowedGroups")?.GetChildren()?.ForEach(item =>
        {
            _allowedGroups.Add(Convert.ToInt64(item.Value));
        });
        _bot = bot;
        _repository = repository;
    }

    public bool CheckGroup(long group_id)
    {
        return _allowedGroups.Contains(group_id);
    }

    [OnRegex(@$"({ME}|{ANY})问(.*)你答(.*)")]
    [OnGroupMessage(Break = true, Priority = 0)]
    public async Task AddQuestion(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroup(evt.GroupId)) return;

        var flag = matchGroups[0].Value;
        var question = matchGroups[1].Value;
        var answer = matchGroups[2].Value;

        var is_admin = _bot.IsSuperUser(evt.UserId) || evt.Sender.Role >= GroupUserRole.Admin;

        await _repository.Add(question, answer, evt.GroupId, evt.UserId);

        await _bot.SendGroupMessageAsync(evt.GroupId, Message.Parse("我记住了~"));
    }

    [OnGroupMessage(Break = true)]
    public async Task MatchQuestion(GroupMessageEvent evt)
    {
        if (!CheckGroup(evt.GroupId)) return;
        var (state, answer) = _repository.MatchText(evt.RawMessage, evt.GroupId, evt.UserId);
        if (state)
        {
            await _bot.SendGroupMessageAsync(evt.GroupId, Message.Parse(answer));
            return;
        }
        (state, answer) = _repository.MatchRegex(evt.RawMessage, evt.GroupId, evt.UserId);
        if (state)
        {
            await _bot.SendGroupMessageAsync(evt.GroupId, Message.Parse(answer));
            return;
        }
    }
}
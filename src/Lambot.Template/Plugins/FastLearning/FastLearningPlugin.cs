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
    private readonly LambotContext _context;
    public FastLearningPlugin(
        IConfiguration configuration,
        Bot bot,
        FastLearningRepository repository,
        LambotContext context)
    {
        configuration.GetSection("AllowedGroups")?.GetChildren()?.ForEach(item =>
        {
            _allowedGroups.Add(Convert.ToInt64(item.Value));
        });
        _bot = bot;
        _repository = repository;
        _context = context;
    }

    public bool CheckGroup(long group_id)
    {
        return _allowedGroups.Contains(group_id);
    }

    [OnRegex(@$"({ME}|{ANY})(问|说)(.*)你(答|说)(.*)")]
    [OnGroupMessage(Break = true, Priority = 0)]
    public async Task<string> AddQuestion(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroup(evt.GroupId)) return null;

        var flag = matchGroups[0].Value;
        var question = matchGroups[2].Value;
        var answer = matchGroups[4].Value;

        var is_admin = _bot.IsSuperUser(evt.UserId) || evt.Sender.Role >= GroupUserRole.Admin;
        switch (flag)
        {
            case ME:
                await _repository.Add(question, answer, evt.GroupId, evt.UserId);
                break;
            case ANY:
                if (!is_admin) return "你没有权限添加有人问~";
                await _repository.Add(question, answer, evt.GroupId, 0);
                break;
            default:
                return "没听明白你在说什么呢~";
        }

        return "我已经记住了~";
    }

    [OnGroupMessage(Break = true)]
    public async Task<string> MatchQuestion(GroupMessageEvent evt)
    {
        if (!CheckGroup(evt.GroupId))
        {
            throw _context.Skip();
        }
        var (state, answer) = _repository.MatchText(evt.RawMessage, evt.GroupId, evt.UserId);
        if (state)
        {
            return await Task.FromResult(answer);
        }
        (state, answer) = _repository.MatchRegex(evt.RawMessage, evt.GroupId, evt.UserId);
        if (state)
        {
            return answer;
        }
        throw _context.Skip();
    }
}
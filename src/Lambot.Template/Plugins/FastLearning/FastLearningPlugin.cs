using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Lambot.Template.Plugins.FastLearning.Entity;
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

    //判断群号是否在白名单
    private bool CheckGroupPermission(long group_id)
    {
        return _allowedGroups.Contains(group_id);
    }

    //设置问答
    [OnRegex(@$"({ME}|{ANY})(问|说)(.*)你(答|说)(.*)")]
    [OnPermission(Role = GroupUserRole.Admin | GroupUserRole.Owner)]
    [OnMessage(Type = MessageType.Group, Priority = 0, Break = true)]
    public async Task<string> AddQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;

        var flag = matchGroups[0].Value;
        var question = matchGroups[2].Value;
        var answer = matchGroups[4].Value;

        var is_admin = _bot.IsSuperUser(evt.UserId) || evt.Sender.Role >= GroupUserRole.Admin;
        switch (flag)
        {
            case ME:
                return await _repository.AddAsync(question, answer, evt.GroupId, evt.UserId);
            case ANY:
                if (!is_admin) return "你没有权限添加有人问~";
                return await _repository.AddAsync(question, answer, evt.GroupId, 0);
            default:
                return "没听明白你在说什么呢~";
        }
    }

    //删除问答
    [OnRegex(@$"删除({ME}|{ANY})(问|说)(.*)")]
    [OnMessage(Type = MessageType.Group, Priority = 0, Break = true)]
    public async Task<string> DelQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;

        var flag = matchGroups[0].Value;
        var question = matchGroups[2].Value;

        var is_admin = _bot.IsSuperUser(evt.UserId) || evt.Sender.Role >= GroupUserRole.Admin;
        switch (flag)
        {
            case ME:
                return await _repository.DelAsync(question, evt.GroupId, evt.UserId);
            case ANY:
                if (!is_admin) return "你没有权限删除有人问~";
                return await _repository.DelAsync(question, evt.GroupId, 0);
            default:
                return "没听明白你在说什么呢~";
        }
    }

    //匹配群消息
    [OnMessage(Type = MessageType.Group, Break = true)]
    public async Task<string> MatchQuestionAsync(GroupMessageEvent evt)
    {
        if (!CheckGroupPermission(evt.GroupId))
        {
            throw _context.Skip();
        }

        var answer = _repository.MatchText(evt.RawMessage, evt.GroupId, evt.UserId);
        if (answer != default)
        {
            return await Task.FromResult(answer);
        }

        answer = _repository.MatchRegex(evt.RawMessage, evt.GroupId, evt.UserId);
        if (answer != default)
        {
            return answer;
        }

        throw _context.Skip();
    }

    [OnRegex(@$"看看({ME}|{ANY})(问|说)")]
    [OnMessage(Type = MessageType.Group, Break = true)]
    public async Task<string> ListQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;

        var flag = matchGroups[0].Value;
        var is_admin = _bot.IsSuperUser(evt.UserId) || evt.Sender.Role >= GroupUserRole.Admin;
        var list = default(List<FastLearningRecord>);
        switch (flag)
        {
            case ME:
                list = await _repository.ListAsync(evt.GroupId, evt.UserId);
                break;
            case ANY:
                list = await _repository.ListAsync(evt.GroupId, 0);
                break;
            default:
                return "没听明白你在说什么呢~";
        }

        if (list is null || list.Count == 0)
        {
            return "一个问题都没有呢~";
        }

        var forward_list = new List<object>();
        foreach (var item in list)
        {
            forward_list.Add(new
            {
                type = "node",
                data = new
                {
                    name = item.UserId == 0 ? "有人问" : "你问",
                    uin = item.UserId == 0 ? evt.SelfId : evt.UserId,
                    content = item.Question
                }
            });
            forward_list.Add(new
            {
                type = "node",
                data = new
                {
                    name = "我答",
                    uin = evt.SelfId,
                    content = item.Answer
                }
            });
        }

        await _bot.CallApi("send_group_forward_msg", new
        {
            group_id = evt.GroupId,
            messages = forward_list
        });
        throw _context.Break();
    }
}
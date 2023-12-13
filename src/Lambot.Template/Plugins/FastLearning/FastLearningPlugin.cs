using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Lambot.Template.Plugins.FastLearning.Entity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.FastLearning;

[PluginInfo(Name = "快速学习")]
public class FastLearningPlugin : PluginBase
{
    private readonly HashSet<long> _allowedGroups = new();
    private readonly OneBotClient _oneBotClient;

    private const string ME = "我";
    private const string ANY = "有人";
    private const string AT = @"\[CQ:at,qq=\d+\]";
    private const string QQ = @"\d+";
    private readonly FastLearningRepository _repository;
    private readonly LambotContext _context;

    public FastLearningPlugin(
        IConfiguration configuration,
        OneBotClient oneBotClient,
        FastLearningRepository repository,
        LambotContext context)
    {
        configuration.GetSection("AllowedGroups").GetChildren().ForEach(item => { _allowedGroups.Add(Convert.ToInt64(item.Value)); });
        _oneBotClient = oneBotClient;
        _repository = repository;
        _context = context;
    }

    //判断群号是否在白名单
    private bool CheckGroupPermission(long group_id)
    {
        return _allowedGroups.Contains(group_id);
    }

    //设置问答
    [OnGroupMessage]
    [OnRegex(@$"\s*({ME}|{ANY})\s*(问|说)\s*(.*)\s*你\s*(答|说)\s*(.*)\s*")]
    public async Task<string?> AddQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;

        var flag = matchGroups[0].Value;
        var question = matchGroups[2].Value;
        var answer = matchGroups[4].Value;

        var is_admin = evt.Sender.Role >= GroupUserRole.Admin;
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
    [OnGroupMessage]
    [OnRegex(@$"\s*删除\s*({ME}|{ANY}|{AT}|{QQ})\s*(问|说)\s*(.*)\s*")]
    public async Task<string?> DelQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;

        var flag = matchGroups[0].Value.Trim();
        var question = matchGroups[2].Value.Trim();

        var is_admin = evt.Sender.Role >= GroupUserRole.Admin;

        if (flag == ME)
        {
            return await _repository.DelAsync(question, evt.GroupId, evt.UserId);
        }
        else if (flag == ANY)
        {
            if (!is_admin) return "你没有权限删除有人问~";
            return await _repository.DelAsync(question, evt.GroupId, 0);
        }
        else
        {
            if (!is_admin) return "你没有权限删除别人的问答哦~";
            if (Regex.IsMatch(flag, AT))
            {
                var seg = MessageSeg.Parse(flag);
                if (seg is AtMessageSeg at_seg)
                {
                    return await _repository.DelAsync(question, evt.GroupId, at_seg.UserId);

                }
            }
            else if (long.TryParse(flag, out var userId))
            {
                return await _repository.DelAsync(question, evt.GroupId, userId);
            }
        }
        return "没听明白你在说什么呢~";
    }

    //匹配群消息
    [OnGroupMessage(Priority = 100)]
    public async Task<string> MatchQuestionAsync(GroupMessageEvent evt)
    {
        if (!CheckGroupPermission(evt.GroupId))
        {
            throw _context.Skip();
        }

        var answer = await _repository.MatchTextAsync(evt.RawMessage, evt.GroupId, evt.UserId);
        if (answer != default)
        {
            return answer;
        }

        answer = _repository.MatchRegex(evt.RawMessage, evt.GroupId, evt.UserId);
        if (answer != default)
        {
            return answer;
        }

        throw _context.Skip();
    }

    [OnGroupMessage]
    [OnRegex(@$"?\s*看看\s*({ME}|{ANY}|{AT}|{QQ})\s*(问|说)\s*")]
    public async Task<string?> ListQuestionAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        if (!CheckGroupPermission(evt.GroupId)) return null;
        var is_admin = evt.Sender.Role >= GroupUserRole.Admin;

        var flag = matchGroups[0].Value.Trim();
        var forward_user_id = 0L;

        var list = new List<FastLearningRecord>(0);
        if (flag == ME)
        {
            forward_user_id = evt.UserId;
            list = await _repository.ListAsync(evt.GroupId, evt.UserId);
        }
        else if (flag == ANY)
        {
            forward_user_id = 0;
            list = await _repository.ListAsync(evt.GroupId, 0);
        }
        else
        {
            if (!is_admin) return "你没有权限查看别人的问答哦~";
            if (Regex.IsMatch(flag, AT))
            {
                var seg = MessageSeg.Parse(flag);
                if (seg is AtMessageSeg at_seg)
                {
                    forward_user_id = at_seg.UserId;
                    list = await _repository.ListAsync(evt.GroupId, at_seg.UserId);
                }
            }
            else if (long.TryParse(flag, out var userId))
            {
                forward_user_id = userId;
                list = await _repository.ListAsync(evt.GroupId, userId);
            }
            else
            {
                return "没听明白你在说什么呢~";
            }
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
                    name = forward_user_id == 0 ? "有人问" : "你问",
                    uin = forward_user_id == 0 ? evt.SelfId : forward_user_id,
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

        await _oneBotClient.SendAsync("send_group_forward_msg", new
        {
            group_id = evt.GroupId,
            messages = forward_list
        });
        throw _context.Break();
    }
}
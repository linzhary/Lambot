using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Lambot.Template.Database;
using Lambot.Template.Database.Entity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace Lambot.Template.Plugins;

[PluginInfo(Name = "常用功能")]
public class CommonPlugin(OneBotClient oneBotClient, LambotContext context, IHttpClientFactory httpClientFactory, LambotDbContext dbContext) : PluginBase
{
    private static readonly object _queryPriceLock = new();
    private static bool _isQueryPriceNow = false;
    [OnGroupMessage]
    [OnCommand("查询价格")]
    public Message QueryPrice(GroupMessageEvent evt)
    {
        if (_isQueryPriceNow) return (Message)"有一个价格正在查询，请稍后再试！";
        lock (_queryPriceLock)
        {
            if (_isQueryPriceNow) return (Message)"有一个价格正在查询，请稍后再试！";
            _isQueryPriceNow = true;
            if (string.IsNullOrWhiteSpace(evt.RawMessage)) return (Message)"参数不正确";
            NativeMethods.SetClipboard(evt.RawMessage);
            
            var exePath = "./Executable/main.exe";
            var psi = new ProcessStartInfo
            {
                RedirectStandardOutput = true, // 重定向标准输出
                FileName = exePath
            };
            using var process = new Process();
            process.StartInfo = psi;
            process.Start();
            var base64 = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            _isQueryPriceNow = false;
            var message = new Message();
            message.Segments.Add(new ImageMessageSeg($"base64://{base64}"));
            return message;
        }
    }


    /// <summary>
    /// 自动强化
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    [OnGroupMessage]
    [OnStartWith("/自动强化")]
    public void AutoRefine(GroupMessageEvent evt)
    {
        var argString = evt.RawMessage.Split(" ").ElementAtOrDefault(1);
        var arg = Convert.ToInt32(argString);
        var userName = string.IsNullOrEmpty(evt.Sender.Card) ? evt.Sender.Nickname : evt.Sender.Card;
        for (var i = 0; ; i++)
        {
            var final = Random.Shared.Next(1, 101);
            if (final <= arg)
            {
                throw context.Finish($"[{userName}] 经过了 [{i + 1}] 手。终于强化成功了!");
            }
        }
    }

    /// <summary>
    /// ROLL点
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    [OnGroupMessage]
    [OnFullMatch("/roll")]
    public async Task RollAsync(GroupMessageEvent evt)
    {
        var userName = string.IsNullOrEmpty(evt.Sender.Card) ? evt.Sender.Nickname : evt.Sender.Card;
        for (var i = 0; ; i++)
        {
            var message = $"[{userName}] 掷出了 {Random.Shared.Next(1, 101)} 点!";
            await oneBotClient.SendGroupMessageAsync(evt.GroupId, message);
        }
    }

    [OnGroupMessage]
    [OnRegex(@"60[秒,s,S]")]
    public async Task<Message?> ZaoBao(GroupMessageEvent evt)
    {
        using var client = httpClientFactory.CreateClient();
        var json = await client.GetStringAsync("https://api.03c3.cn/api/zb?type=jsonImg");
        var imageUrl = JObject.Parse(json)["data"]?.Value<string>("imageurl");
        var datetime = JObject.Parse(json)["data"]?.Value<string>("datetime");
        if (!string.IsNullOrEmpty(datetime) && !string.IsNullOrEmpty(imageUrl))
        {
            var message = new Message();
            message.Segments.Add(new ImageMessageSeg(imageUrl));
            return message;
        }
        return (Message)"获取数据失败";
    }

    [OnGroupMessage(Priority = 0, Break = false)]
    public async Task OnMessageAsync(GroupMessageEvent evt)
    {
        if (evt.UserId == evt.SelfId) return;
        if (evt.Message is null) return;
        await dbContext.MessageHistories.AddAsync(new MessageHistory
        {
            MessageId = evt.MessageId,
            GroupId = evt.GroupId,
            UserId = evt.UserId,
            RawMessage = evt.RawMessage,
            Time = evt.Time,
        });

        var hs = new HashSet<long>();
        foreach (var seg in evt.Message.Segments)
        {
            if (seg is AtMessageSeg atMessageSeg && hs.Add(atMessageSeg.UserId))
            {
                await dbContext.AtMessageHistories.AddAsync(new AtMessageHistory
                {
                    MessageId = evt.MessageId,
                    GroupId = evt.GroupId,
                    UserId = atMessageSeg.UserId
                });
            }
        }

        await dbContext.SaveChangesAsync();
    }

    [OnGroupMessage]
    [OnFullMatch("谁艾特我")]
    public async Task WhoAtMeAsync(GroupMessageEvent evt)
    {
        var messageIds = await dbContext.AtMessageHistories
            .Where(x => x.GroupId == evt.GroupId)
            .Where(x => x.UserId == evt.UserId)
            .Select(x => x.MessageId)
            .Take(10)
            .ToListAsync();

        if (messageIds.Count == 0)
            throw context.Skip();

        var forward_list = messageIds.Select(x => new
        {
            type = "node",
            data = new
            {
                id = x
            }
        }).ToList();

        await oneBotClient.SendAsync("send_group_forward_msg", new
        {
            group_id = evt.GroupId,
            messages = forward_list
        });
    }

    [OnGroupMessage]
    [OnFullMatch("谁艾特我1")]
    public async Task<Message> WhoAtMe1Async(GroupMessageEvent evt)
    {
        var messageId = await dbContext.AtMessageHistories
            .Where(x => x.GroupId == evt.GroupId)
            .Where(x => x.UserId == evt.UserId)
            .Select(x => x.MessageId)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync();

        if (messageId == default)
            throw context.Skip();

        var message = new Message();
        message.Segments.Add(new ReplyMessageSeg(messageId));
        message.Segments.Add(new AtMessageSeg(evt.UserId));

        return message;
    }

    //[OnNotice(Type = NoticeType.GroupRecall)]
    public async Task<string?> OnGroupRecallAsync(GroupNoticeEvent evt)
    {
        if (evt.UserId == evt.SelfId) return default;
        var messageId = evt.RawMessage.Value<long>("message_id");
        var message = await dbContext.MessageHistories.Where(x => x.MessageId == messageId).FirstOrDefaultAsync();
        if (message == null) return default;
        await oneBotClient.SendAsync("get_group_member_list", new
        {
            group_id = evt.GroupId
        }, async ret =>
        {
            if (ret.Value<int>("retcode") is not 0) return;
            var user_name = ret["data"]!.Children().Where(x => x.Value<long>("user_id") == evt.UserId)
            .Select(x => string.IsNullOrEmpty(x.Value<string>("card")) ? x.Value<string>("nickname") : x.Value<string>("card"))
            .FirstOrDefault();
            var forward_list = new List<object>()
            {
                    new
                {
                    type = "node",
                    data = new
                    {
                        name = user_name,
                        uin = message.UserId,
                        content = $"[{user_name}] 撤回的消息如下："
                    }
                },
                new
                {
                    type = "node",
                    data = new
                    {
                        name = user_name,
                        uin = message.UserId,
                        content = message.RawMessage
                    }
                }
            };
            await oneBotClient.SendAsync("send_group_forward_msg", new
            {
                group_id = evt.GroupId,
                messages = forward_list
            });

        });
        throw context.Skip();
    }
}
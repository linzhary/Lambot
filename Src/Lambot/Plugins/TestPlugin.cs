 using Lambot.Adapters.OneBot;
using Lambot.Adapters.OneBot.RuleMatchers;
using Lambot.Adapters.OneBot.TypeMatchers;
using Lambot.Plugin;
using Microsoft.Extensions.Logging;

namespace Lambot.Plugins;

public class TestPlugin : LambotPlugin
{
    private readonly Bot _bot;
    private readonly ILogger<TestPlugin> _logger;

    public TestPlugin(ILogger<TestPlugin> logger, Bot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    //所有消息
    [OnMessage(Priority = 99)]
    public async Task OnMessage(MessageEvent evt)
    {
        if(evt is GroupMessageEvent groupMessageEvent)
        {
            var reply = new Message();
            reply.Segments.Add(MessageSegment.Text($"你发送了：{evt.RawMessage}"));
            await _bot.SendGroupMessageAsync(groupMessageEvent.GroupId, reply);
        }
    }
    
    /// <summary>
    /// 群聊消息
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    [OnGroupMessage(Priority = 98, Break = true)]
    public async Task OnGroupMessage(GroupMessageEvent evt)
    {
        var message = Message.Parse(evt.RawMessage);
        foreach (var seg in message.Segments)
        {
            if (seg.Type == "at")
            {
                seg.Props["qq"] = 2724078466;
            }
        }
        await _bot.SendGroupMessageAsync(evt.GroupId, message);
    }

    /// <summary>
    /// 群聊消息，完全匹配 哈哈哈 的
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    [OnFullMatch("哈哈哈")]
    [OnGroupMessage(Priority = 97, Break = true)]
    public async Task OnGroupMessage1(GroupMessageEvent evt)
    {
        var reply = new Message();
        reply.Segments.Add(MessageSegment.Text($"哈哈哈：{evt.RawMessage}"));
        await _bot.SendGroupMessageAsync(evt.GroupId, reply);
    }

    /// <summary>
    /// 私聊消息
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    [OnPrivateMessage(Priority = 99)]
    public async Task OnPrivateMessage(PrivateMessageEvent evt)
    {
        var reply = new Message();
        reply.Segments.Add(MessageSegment.Text($"你发送了：{evt.RawMessage}"));
        await _bot.SendPrivateMessageAsync(evt.UserId, reply);
    }
}

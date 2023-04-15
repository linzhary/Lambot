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

    [OnMessage(Priority = 99, Break = true)]
    public async Task OnMessage(MessageEvent evt)
    {
        if(evt is GroupMessageEvent groupMessageEvent)
        {
            var reply = new Message();
            reply.Segments.Add(MessageSegment.Text($"你发送了：{evt.RawMessage}"));
            await _bot.SendGroupMessageAsync(groupMessageEvent.GroupId, reply);
        }
    }
    
    [OnGroupMessage(Priority = 98, Break = true)]
    public async Task OnGroupMessage(GroupMessageEvent evt)
    {
        var reply = new Message();
        reply.Segments.Add(MessageSegment.Text($"OnGroupMessage：{evt.RawMessage}"));
        await _bot.SendGroupMessageAsync(evt.GroupId, reply);
    }

    [OnFullMatch("OnFullMatch")]
    [OnGroupMessage(Priority = 97, Break = true)]
    public async Task OnGroupMessage1(GroupMessageEvent evt)
    {
        var reply = new Message();
        reply.Segments.Add(MessageSegment.Text($"OnFullMatch：{evt.RawMessage}"));
        await _bot.SendGroupMessageAsync(evt.GroupId, reply);
    }

    [OnPrivateMessage(Priority = 99)]
    public async Task OnPrivateMessage(PrivateMessageEvent evt)
    {
        var reply = new Message();
        reply.Segments.Add(MessageSegment.Text($"你发送了：{evt.RawMessage}"));
        await _bot.SendPrivateMessageAsync(evt.UserId, reply);
    }
}

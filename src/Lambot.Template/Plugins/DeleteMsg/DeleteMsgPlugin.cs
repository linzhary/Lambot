using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;


[PluginInfo(Name = "撤回消息")]
public class DeleteMsgPlugin : PluginBase
{
    private readonly OneBotClient _oneBotClient;
    private readonly LambotContext _context;

    public DeleteMsgPlugin(OneBotClient oneBotClient, LambotContext context)
    {
        _oneBotClient = oneBotClient;
        _context = context;
    }

    [OnEndsWith("撤回")]
    [OnMessage(Type = MessageType.Group, Priority = 0)]
    public async Task DeleteMsgAsync(GroupMessageEvent evt)
    {
        var seg_0 = evt.Message?.Segments.ElementAtOrDefault(0);
        if (seg_0 is null) return;
        if (seg_0 is not ReplyMessageSeg reply_seg) return;
        var seg_1 = evt.Message?.Segments.ElementAtOrDefault(1);
        if (seg_1 is null) return;
        if (seg_1 is not AtMessageSeg) return;

        await _oneBotClient.SendAsync("delete_msg", new
        {
            message_id = reply_seg.Id
        });
        throw _context.Break();
    }
}
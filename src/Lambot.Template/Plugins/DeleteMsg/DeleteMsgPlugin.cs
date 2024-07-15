using System.Text;
using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[PluginInfo(Name = "撤回消息", Enabled = false)]
public class DeleteMsgPlugin : PluginBase
{
    private readonly OneBotClient _oneBotClient;
    private readonly LambotContext _context;

    private readonly IHttpClientFactory httpClientFactory;

    public DeleteMsgPlugin(OneBotClient oneBotClient, LambotContext context, IHttpClientFactory httpClientFactory)
    {
        _oneBotClient = oneBotClient;
        _context = context;
        this.httpClientFactory = httpClientFactory;
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

    // [OnStartWith(".chat")]
    // [OnMessage(Type = MessageType.Group, Priority = -1, Break = true)]
    // public async Task<Message> ChatAsync(GroupMessageEvent evt)
    // {
    //     using var client = new HttpClient();
    //     // 设置 API 密钥
    //     client.DefaultRequestHeaders.Add("Authorization", "Bearer " + "");

    //     // 构造请求体
    //     var requestData = new
    //     {
    //         model = "gpt-3.5-turbo",
    //         messages = new[]
    //         {
    //             new {
    //                 role = "system",
    //                 content = "You are a proud and charming young lady.",
    //             },
    //             new {
    //                 role = "user",
    //                 content = evt.RawMessage.Replace(".chat", string.Empty),
    //             }

    //         }
    //     };

    //     var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

    //     // 发送请求
    //     var response = await client.PostAsync("https://api.openai-proxy.com/v1/chat/completions", content);

    //     // 获取响应内容
    //     var responseString = await response.Content.ReadAsStringAsync();

    //     var jObj = JObject.Parse(responseString);
    //     var reply = new Message();
    //     reply.Segments.Add(new ReplyMessageSeg(evt.MessageId));
    //     reply.Segments.Add(new TextMessageSeg { Text = jObj["choices"]![0]!["message"]!.Value<string>("content")! });
    //     return reply;
    // }
}
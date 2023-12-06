using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.Dice;

[PluginInfo(Name = "骰弟")]
public class DicePlugin : PluginBase
{
    private readonly OneBotClient _oneBotClient;
    private readonly LambotContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public DicePlugin(OneBotClient oneBotClient, LambotContext context, IHttpClientFactory httpClientFactory)
    {
        _oneBotClient = oneBotClient;
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [OnRegex(@"\.[rR][dD]")]
    [OnMessage(Type = MessageType.Group, Priority = -1, Break = true)]
    public string? Dice16Async(GroupMessageEvent evt, Group[] matchGroups)
    {
        return DiceAsync(1, 6);
    }

    [OnRegex(@"\.[rR]([-+]?\d+\.?\d*)[dD]")]
    [OnMessage(Type = MessageType.Group, Priority = -1, Break = true)]
    public string? DiceN6Async(GroupMessageEvent evt, Group[] matchGroups)
    {
        var diceCount = int.Parse(matchGroups[0].Value);
        return DiceAsync(diceCount, 6);
    }

    [OnRegex(@"\.[rR]([-+]?\d+\.?\d*)[dD]([-+]?\d+\.?\d*)")]
    [OnMessage(Type = MessageType.Group, Priority = 0, Break = true)]
    public string? DiceNNAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        var diceCount = int.Parse(matchGroups[0].Value);
        var faceCount = int.Parse(matchGroups[1].Value);
        return DiceAsync(diceCount, faceCount);
    }

    private string? DiceAsync(int diceCount, int faceCount)
    {
        if (diceCount < 1 || faceCount < 1)
        {
            return $"你想扔【{diceCount}】个【{faceCount}】面的骰子?, 早点睡吧";
        }
        if (diceCount > 999 || faceCount > 999)
        {
            return $"你想扔【{diceCount}】个【{faceCount}】面的骰子?, 放过我吧";
        }
        var result = 0;
        var random = new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        for (int i = 0; i < diceCount; i++)
        {
            result += random.Next(1, faceCount);
        }
        return $"你扔了【{diceCount}】个【{faceCount}】面的骰子, 结果是【{result}】";
    }

    [OnRegex(@"60[秒,s,S]")]
    [OnMessage(Type = MessageType.Group, Priority = 0, Break = true)]
    public async Task<Message?> ZaoBao(GroupMessageEvent evt)
    {
        using var client = _httpClientFactory.CreateClient();
        var json = await client.GetStringAsync("https://api.03c3.cn/api/zb?type=jsonImg");
        var imageUrl = JObject.Parse(json)["data"]?.Value<string>("imageurl");
        var datetime = JObject.Parse(json)["data"]?.Value<string>("datetime");
        if (!string.IsNullOrEmpty(datetime) && !string.IsNullOrEmpty(imageUrl))
        {
            var message = new Message();
            message.Segments.Add(new ImageMessageSeg(datetime, imageUrl));
            return message;
        }
        return (Message)"获取数据失败";
    }
}
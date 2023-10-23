using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.Dice;

[PluginInfo(Name = "骰弟")]
public class DicePlugin : PluginBase
{
    private readonly OneBotClient _oneBotClient;
    private readonly LambotContext _context;

    public DicePlugin(OneBotClient oneBotClient, LambotContext context)
    {
        _oneBotClient = oneBotClient;
        _context = context;
    }

    [OnRegex(@"\.[rR][dD]")]
    [OnMessage(Type = MessageType.Group, Priority = -1, Break = true)]
    public string? Dice16Async(GroupMessageEvent evt, Group[] matchGroups)
    {
        return DiceAsync(1,6);
    }

    [OnRegex(@"\.[rR]([-+]?\d+\.?\d*)[dD]")]
    [OnMessage(Type = MessageType.Group, Priority = -1, Break = true)]
    public string? DiceN6Async(GroupMessageEvent evt, Group[] matchGroups)
    {
        var diceCount = int.Parse(matchGroups[0].Value);
        return DiceAsync(diceCount,6);
    }

    [OnRegex(@"\.[rR]([-+]?\d+\.?\d*)[dD]([-+]?\d+\.?\d*)")]
    [OnMessage(Type = MessageType.Group, Priority = 0, Break = true)]
    public string? DiceNNAsync(GroupMessageEvent evt, Group[] matchGroups)
    {
        var diceCount = int.Parse(matchGroups[0].Value);
        var faceCount = int.Parse(matchGroups[1].Value);
        return DiceAsync(diceCount,faceCount);
    }

    private string? DiceAsync(int diceCount, int faceCount){
        if(diceCount < 1 || faceCount < 1){
            return $"你想扔【{diceCount}】个【{faceCount}】面的骰子?, 早点睡吧";
        }
        if(diceCount > 999 || faceCount > 999){
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
}
using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public sealed class OnContains : RuleMatcher
{
    private readonly string _text;

    public OnContains(string text)
    {
        _text = text;
        if (Priority == int.MaxValue)
        {
            Priority = int.MaxValue - 1;
        }
    }

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (string.IsNullOrWhiteSpace(messageEvent.RawMessage)) return false;
        return messageEvent.RawMessage.Contains(_text);
    }
}
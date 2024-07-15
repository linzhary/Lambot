using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public sealed class OnStartWith : RuleMatcher
{
    private readonly string _text;

    public OnStartWith(string text)
    {
        _text = text;
        if (Priority == int.MaxValue)
        {
            Priority = int.MaxValue - 3;
        }
    }

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (string.IsNullOrWhiteSpace(messageEvent.RawMessage)) return false;
        return messageEvent.RawMessage.StartsWith(_text);
    }
}
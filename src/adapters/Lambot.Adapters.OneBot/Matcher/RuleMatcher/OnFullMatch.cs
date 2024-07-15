using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnFullMatch : RuleMatcher
{
    private readonly string _text;

    public OnFullMatch(string text)
    {
        _text = text;
    }

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (string.IsNullOrWhiteSpace(messageEvent.RawMessage)) return false;
        return messageEvent.RawMessage == _text;
    }
}
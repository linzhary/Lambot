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
        if (string.IsNullOrWhiteSpace(evt.RawMessage)) return false;
        return evt.RawMessage == _text;
    }
}
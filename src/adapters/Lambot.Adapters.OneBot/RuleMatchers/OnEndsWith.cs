using Lambot.Core;
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public sealed class OnEndsWith : RuleMatcher
{
    private readonly string _text;

    public OnEndsWith(string text)
    {
        _text = text;
        if (Priority == int.MaxValue)
        {
            Priority = int.MaxValue - 2;
        }
    }

    public override bool Check(LambotEvent evt)
    {
        if (string.IsNullOrWhiteSpace(evt.RawMessage)) return false;
        return evt.RawMessage.EndsWith(_text);
    }
}
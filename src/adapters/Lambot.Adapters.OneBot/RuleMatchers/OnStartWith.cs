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
        if (string.IsNullOrWhiteSpace(evt.RawMessage)) return false;
        return evt.RawMessage.StartsWith(_text);
    }
}
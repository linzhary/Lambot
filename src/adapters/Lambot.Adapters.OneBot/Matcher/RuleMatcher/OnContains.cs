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
        if (string.IsNullOrWhiteSpace(evt.RawMessage)) return false;
        return evt.RawMessage.Contains(_text);
    }
}
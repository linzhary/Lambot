using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnEndsWith : RuleMatcher
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

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return raw_message.EndsWith(_text);
    }
}
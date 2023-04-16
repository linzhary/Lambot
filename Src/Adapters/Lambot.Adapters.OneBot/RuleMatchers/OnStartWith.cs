using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnStartWith : RuleMatcher
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

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return raw_message.StartsWith(_text);
    }
}
using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnFullMatch : RuleMatcher
{
    private readonly string _text;

    public OnFullMatch(string text)
    {
        _text = text;
    }

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return raw_message == _text;
    }
}
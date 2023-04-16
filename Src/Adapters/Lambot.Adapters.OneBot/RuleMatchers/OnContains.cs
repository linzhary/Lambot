using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnContains : RuleMatcherAttribute
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

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return raw_message.Contains(_text);
    }
}
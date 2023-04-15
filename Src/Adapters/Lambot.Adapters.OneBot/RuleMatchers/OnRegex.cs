using Lambot.Plugin;

namespace Lambot.Adapters.OneBot.RuleMatchers;
public class OnRegex : RuleMatcherAttribute
{
    private readonly string _pattern;
    public OnRegex(string pattern)
    {
        _pattern = pattern;
    }

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return raw_message.Contains(_pattern);
    }
}

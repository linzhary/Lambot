using Lambot.Plugin;
using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot.RuleMatchers;
public class OnRegex : RuleMatcherAttribute
{
    private readonly string _pattern;
    public OnRegex(string pattern)
    {
        _pattern = _pattern.TrimStart('^').TrimEnd('$');
    }

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        return Regex.IsMatch(raw_message, $"^{_pattern}$");
    }
}

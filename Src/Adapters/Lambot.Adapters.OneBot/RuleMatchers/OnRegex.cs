using Lambot.Core.Plugin;
using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot;

public class OnRegex : RuleMatcher
{
    private readonly string _pattern;
    internal Match MatchResult { get; private set; }

    public OnRegex(string pattern)
    {
        _pattern = pattern.TrimStart('^').TrimEnd('$');
    }

    public override bool Matched(string raw_message)
    {
        if (string.IsNullOrWhiteSpace(raw_message)) return false;
        MatchResult = Regex.Match(raw_message, $"^{_pattern}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return MatchResult.Success;
    }
}
using Lambot.Core.Plugin;
using System.Text.RegularExpressions;
using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public class OnRegex : RuleMatcher
{
    private readonly string _pattern;
    internal Match MatchResult { get; private set; }

    public OnRegex(string pattern)
    {
        _pattern = pattern.TrimStart('^').TrimEnd('$');
    }

    public override bool Check(LambotEvent evt)
    {
        if (string.IsNullOrWhiteSpace(evt.RawMessage)) return false;
        MatchResult = Regex.Match(evt.RawMessage, $"^{_pattern}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return MatchResult.Success;
    }
}
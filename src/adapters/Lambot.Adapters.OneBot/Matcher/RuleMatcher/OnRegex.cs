using Lambot.Core.Plugin;
using System.Text.RegularExpressions;
using Lambot.Core;
using System.Diagnostics.CodeAnalysis;

namespace Lambot.Adapters.OneBot;

public class OnRegex : RuleMatcher
{
    private readonly string _pattern;
    internal Match? MatchResult { get; private set; }

    public OnRegex([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        _pattern = pattern.TrimStart('^').TrimEnd('$');
    }

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (string.IsNullOrWhiteSpace(messageEvent.RawMessage)) return false;
        MatchResult = Regex.Match(messageEvent.RawMessage, $"^{_pattern}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return MatchResult.Success;
    }
}
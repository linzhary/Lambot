using Lambot.Core;

namespace Lambot.Plugin;

public class PluginMatcherParameter
{
    public TypeMatcherAttribute TypeMatcher { get; set; }
    public RuleMatcherAttribute RuleMatcher { get; set; }
    public LambotPluginInfo PluginInfo { get; set; }
    public LambotEvent Event { get; set; }
    public object PluginInstance { get; set; }

    public int MatchType => TypeMatcher.Type;

    public bool IsRuleMatched
    {
        get
        {
            if (RuleMatcher is null) return true;
            return RuleMatcher.Matched(Event.RawMessage);
        }
    }
}
public interface IPluginMatcher
{
    void Invoke(PluginMatcherParameter parameter);
}

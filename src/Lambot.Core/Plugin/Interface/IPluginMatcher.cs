﻿using System.Reflection;

namespace Lambot.Core.Plugin;

public class PluginMatcherParameter
{
    public TypeMatcher TypeMatcher { get; set; }
    public RuleMatcher RuleMatcher { get; set; }
    public MethodInfo MethodInfo { get; set; }
    public PluginInfo PluginInfo { get; set; }
    public LambotEvent Event { get; set; }
    public object PluginInstance { get; set; }

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
using System.Reflection;

namespace Lambot.Core.Plugin;

public class PluginMatcherParameter
{
    public TypeMatcher? TypeMatcher { get; internal set; } = null;
    public RuleMatcher? RuleMatcher { get; internal set; } = null;
    public PermMatcher? PermMatcher { get; internal set; } = null;
    public MethodInfo MethodInfo { get; internal set; } = null!;
    public PluginInfo PluginInfo { get; internal set; } = null!;
    public LambotEvent Event { get; internal set; } = null!;
    public object PluginInstance { get; internal set; } = null!;

    public bool IsTypeChecked
    {
        get
        {
            if (TypeMatcher is null) return true;
            return TypeMatcher.Check(Event);
        }
    }

    public bool IsRuleChecked
    {
        get
        {
            if (RuleMatcher is null) return true;
            return RuleMatcher.Check(Event);
        }
    }

    public bool IsPermChecked
    {
        get
        {
            if (PermMatcher is null) return true;
            return PermMatcher.Check(Event);
        }
    }
}

/// <summary>
/// 应当以Scope方式注入
/// </summary>
public interface IPluginMatcher
{
    Task InvokeAsync(PluginMatcherParameter parameter);
}
namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class TypeMatcherAttribute : Attribute
{
    public readonly string Id = Guid.NewGuid().ToString("n");

    /// <summary>
    /// 匹配类型
    /// </summary>
    public abstract int Type { get; }

    /// <summary>
    /// 优先级，值越小优先级越高，默认为<see cref="int.MaxValue"/>
    /// </summary>
    public virtual int Priority { get; set; } = int.MaxValue;

    /// <summary>
    /// 是否阻断消息
    /// </summary>
    public bool Break { get; set; } = false;

    /// <summary>
    /// RuleMatcher的优先级
    /// </summary>
    public int RulePrioirty { get; set; } = int.MaxValue;
}
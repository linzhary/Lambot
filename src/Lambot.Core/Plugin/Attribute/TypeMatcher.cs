namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class TypeMatcher : Attribute
{
    public readonly string Id = Guid.NewGuid().ToString("n");

    /// <summary>
    /// 优先级，值越小优先级越高，默认为100
    /// </summary>
    public virtual int Priority { get; set; } = 100;

    /// <summary>
    /// 是否阻断消息
    /// </summary>
    public bool Break { get; set; } = false;

    public abstract bool Check(LambotEvent evt);
}
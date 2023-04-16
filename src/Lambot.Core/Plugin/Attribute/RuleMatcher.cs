namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class RuleMatcher : Attribute
{
    public readonly string Id = Guid.NewGuid().ToString("n");

    /// <summary>
    /// 优先级，值越小优先级越高，默认为<see cref="int.MaxValue"/>
    /// </summary>
    public virtual int Priority { get; set; } = int.MaxValue;

    /// <summary>
    /// 匹配方法实现
    /// </summary>
    /// <param name="raw_message"></param>
    /// <returns></returns>
    public abstract bool Matched(string raw_message);
}
namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class PermMatcher: Attribute
{
    public readonly string Id = Guid.NewGuid().ToString("n");

    public abstract bool Check(LambotEvent evt);
}
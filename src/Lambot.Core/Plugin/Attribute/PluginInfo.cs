namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PluginInfo : Attribute
{
    /// <summary>
    /// 插件名称
    /// </summary>
    public string? Name { get; set; } = null;

    /// <summary>
    /// 插件描述
    /// </summary>
    public string? Description { get; set; } = null;

    /// <summary>
    /// 插件版本
    /// </summary>
    public string? Version { get; set; } = "1.0.0";
}
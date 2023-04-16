using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Core.Plugin;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class PluginAttribute : Attribute
{
    /// <summary>
    /// 插件名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 插件描述
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 插件版本
    /// </summary>
    public string Version { get; set; } = "1.0.0";
}

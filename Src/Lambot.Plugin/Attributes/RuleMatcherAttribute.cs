using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Plugin;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public abstract class RuleMatcherAttribute : Attribute
{
    public readonly string Id = Guid.NewGuid().ToString("n");
    public virtual int Priority { get; set; } = int.MaxValue;
    public abstract bool Matched(string raw_message);
}

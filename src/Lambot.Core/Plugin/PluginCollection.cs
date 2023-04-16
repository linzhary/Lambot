using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Lambot.Core.Plugin;

internal class PluginCollection : IPluginCollection
{
    internal static List<TypeMatcher> _typeMatcherList = new();
    internal static readonly ConcurrentDictionary<string, PluginBase> _pluginAttrMap = new();
    internal static readonly ConcurrentDictionary<string, RuleMatcher> _ruleMatcherMap = new();
    internal static readonly ConcurrentDictionary<string, PluginTypeInfo> _pluginInfoMap = new();
    private readonly IPluginMatcher _pluginMatcher;
    private readonly LambotContext _context;
    private readonly IServiceProvider _serviceProvider;

    public PluginCollection(IPluginMatcher pluginMatcher, LambotContext context, IServiceProvider serviceProvider)
    {
        _pluginMatcher = pluginMatcher;
        _context = context;
        _serviceProvider = serviceProvider;
    }

    internal static void TryAdd(Type pluginType, MethodInfo methodInfo)
    {
        var typeMatcher = methodInfo.GetCustomAttribute<TypeMatcher>();
        if (typeMatcher is null) return;
        var ruleMatcher = methodInfo.GetCustomAttribute<RuleMatcher>();
        if (ruleMatcher is not null)
        {
            typeMatcher.RulePrioirty = ruleMatcher.Priority;
            _ruleMatcherMap.TryAdd(typeMatcher.Id, ruleMatcher);
        }
        _pluginInfoMap.TryAdd(typeMatcher.Id, new PluginTypeInfo
        {
            Type = pluginType,
            MethodName = methodInfo.Name,
        });
        _typeMatcherList.Add(typeMatcher);
        _typeMatcherList = _typeMatcherList
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.RulePrioirty)
            .ThenBy(x => x.Id)
            .ToList();
    }

    public void OnMessageAsync(LambotEvent evt)
    {
        foreach (var typeMatcher in _typeMatcherList)
        {
            var pluginInfo = _pluginInfoMap.GetValueOrDefault(typeMatcher.Id);
            var parameter = new PluginMatcherParameter
            {
                TypeMatcher = typeMatcher,
                RuleMatcher = _ruleMatcherMap.GetValueOrDefault(typeMatcher.Id),
                PluginInfo = pluginInfo,
                PluginInstance = _serviceProvider.GetRequiredService(pluginInfo.Type),
                Event = evt
            };
            _pluginMatcher.Invoke(parameter);
            if (_context.IsBreak) break;
        }
    }
}
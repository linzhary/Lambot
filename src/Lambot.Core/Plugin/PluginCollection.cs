using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Lambot.Core.Plugin;

internal class PluginCollection : IPluginCollection
{
    internal static List<TypeMatcher> _typeMatcherList = new();
    internal static readonly ConcurrentDictionary<string, PluginInfo> _pluginInfoMap = new();
    internal static readonly ConcurrentDictionary<string, RuleMatcher> _ruleMatcherMap = new();
    internal static readonly ConcurrentDictionary<string, PluginTypeInfo> _pluginTypeInfoMap = new();
    private readonly IPluginMatcher _pluginMatcher;
    private readonly LambotContext _context;
    private readonly IServiceProvider _serviceProvider;

    public PluginCollection(IPluginMatcher pluginMatcher, LambotContext context, IServiceProvider serviceProvider)
    {
        _pluginMatcher = pluginMatcher;
        _context = context;
        _serviceProvider = serviceProvider;
    }

    internal static void TryAdd(Type pluginType, PluginInfo pluginInfo, MethodInfo methodInfo)
    {
        var typeMatcher = methodInfo.GetCustomAttribute<TypeMatcher>();
        if (typeMatcher is null) return;
        _pluginInfoMap.TryAdd(typeMatcher.Id, pluginInfo);
        var ruleMatcher = methodInfo.GetCustomAttribute<RuleMatcher>();
        if (ruleMatcher is not null)
        {
            typeMatcher.RulePrioirty = ruleMatcher.Priority;
            _ruleMatcherMap.TryAdd(typeMatcher.Id, ruleMatcher);
        }
        _pluginTypeInfoMap.TryAdd(typeMatcher.Id, new PluginTypeInfo
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
            var pluginInfo = _pluginTypeInfoMap.GetValueOrDefault(typeMatcher.Id);
            var parameter = new PluginMatcherParameter
            {
                TypeMatcher = typeMatcher,
                RuleMatcher = _ruleMatcherMap.GetValueOrDefault(typeMatcher.Id),
                PluginInfo = _pluginInfoMap.GetValueOrDefault(typeMatcher.Id),
                PluginTypeInfo = pluginInfo,
                PluginInstance = _serviceProvider.GetRequiredService(pluginInfo.Type),
                Event = evt
            };
            _pluginMatcher.Invoke(parameter);
            if (_context.IsBreak) break;
        }
    }
}
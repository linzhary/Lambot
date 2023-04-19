using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Lambot.Core.Plugin;

internal class PluginCollection : IPluginCollection
{
    internal static List<TypeMatcher> _typeMatcherList = new();
    internal static readonly ConcurrentDictionary<string, PluginInfo> _pluginInfoMap = new();
    internal static readonly ConcurrentDictionary<string, RuleMatcher> _ruleMatcherMap = new();
    internal static readonly ConcurrentDictionary<string, MethodInfo> _methodInfoMap = new();

    private readonly IServiceProvider _rootRerviceProvier;

    public PluginCollection(IServiceProvider rootServiceProvier)
    {
        _rootRerviceProvier = rootServiceProvier;
    }

    internal static void TryAdd(PluginInfo pluginInfo, MethodInfo methodInfo)
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
        _methodInfoMap.TryAdd(typeMatcher.Id, methodInfo);
        _typeMatcherList.Add(typeMatcher);
        _typeMatcherList = _typeMatcherList
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.RulePrioirty)
            .ThenBy(x => x.Id)
            .ToList();
    }

    public async Task OnMessageAsync(LambotEvent evt)
    {
        using var scope = _rootRerviceProvier.CreateAsyncScope();
        var pluginMatcher = scope.ServiceProvider.GetRequiredService<IPluginMatcher>();
        foreach (var typeMatcher in _typeMatcherList)
        {
            var methodInfo = _methodInfoMap.GetValueOrDefault(typeMatcher.Id);
            if(methodInfo?.DeclaringType is null) continue;
            var parameter = new PluginMatcherParameter
            {
                Event = evt,
                MethodInfo = methodInfo,
                TypeMatcher = typeMatcher,
                PluginInfo = _pluginInfoMap.GetValueOrDefault(typeMatcher.Id),
                RuleMatcher = _ruleMatcherMap.GetValueOrDefault(typeMatcher.Id),
                Context = scope.ServiceProvider.GetRequiredService<LambotContext>(),
                PluginInstance = scope.ServiceProvider.GetRequiredService(methodInfo.DeclaringType)
            };
            await pluginMatcher.InvokeAsync(parameter);
            if (parameter.Context.IsBreaked) break;
        }
    }
}
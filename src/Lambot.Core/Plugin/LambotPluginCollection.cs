﻿using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;

namespace Lambot.Core.Plugin;

internal class LambotPluginCollection : IPluginCollection
{
    internal static List<TypeMatcher> _typeMatcherList = new();
    internal static readonly ConcurrentDictionary<string, PluginInfo> _pluginInfoMap = new();
    internal static readonly ConcurrentDictionary<string, RuleMatcher> _ruleMatcherMap = new();
    internal static readonly ConcurrentDictionary<string, PermMatcher> _permMatcherMap = new();
    internal static readonly ConcurrentDictionary<string, MethodInfo> _methodInfoMap = new();

    private readonly IServiceProvider _rootRerviceProvier;

    public LambotPluginCollection(IServiceProvider rootServiceProvier)
    {
        _rootRerviceProvier = rootServiceProvier;
    }

    internal static void TryAdd(PluginInfo pluginInfo, MethodInfo methodInfo)
    {
        var typeMatcher = methodInfo.GetCustomAttribute<TypeMatcher>();
        if (typeMatcher is null) return;

        _typeMatcherList.Add(typeMatcher);
        _typeMatcherList = _typeMatcherList
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Id)
            .ToList();

        _pluginInfoMap.TryAdd(typeMatcher.Id, pluginInfo);
        _methodInfoMap.TryAdd(typeMatcher.Id, methodInfo);

        if (methodInfo.IsDefined(typeof(RuleMatcher), true))
        {
            _ruleMatcherMap.TryAdd(typeMatcher.Id, methodInfo.GetCustomAttribute<RuleMatcher>()!);
        }
        if (methodInfo.IsDefined(typeof(PermMatcher), true))
        {
            _permMatcherMap.TryAdd(typeMatcher.Id, methodInfo.GetCustomAttribute<PermMatcher>()!);
        }
    }

    public Task OnReceiveAsync(long client_id, LambotEvent evt)
    {
        return Task.Run(async () =>
        {
            using var scope = _rootRerviceProvier.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<LambotContext>();
            context.ClientId = client_id;
            var pluginMatcher = scope.ServiceProvider.GetRequiredService<IPluginMatcher>();
            foreach (var typeMatcher in _typeMatcherList)
            {
                var methodInfo = _methodInfoMap[typeMatcher.Id];
                if (methodInfo.DeclaringType is null) continue;
                var parameter = new PluginMatcherParameter
                {
                    Event = evt,
                    MethodInfo = methodInfo,
                    TypeMatcher = typeMatcher,
                    PluginInfo = _pluginInfoMap[typeMatcher.Id],
                    RuleMatcher = _ruleMatcherMap.GetValueOrDefault(typeMatcher.Id),
                    PermMatcher = _permMatcherMap.GetValueOrDefault(typeMatcher.Id),
                    PluginInstance = scope.ServiceProvider.GetRequiredService(methodInfo.DeclaringType)
                };
                await pluginMatcher.InvokeAsync(parameter);
                if (context.IsBreaked) break;
            }
        });
    }
}
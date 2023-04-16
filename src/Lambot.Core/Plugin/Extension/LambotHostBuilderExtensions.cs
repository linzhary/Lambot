using Lambot.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lambot.Core;

public static class LambotHostBuilderExtensions
{
    public static void RegisterPlugins(this LambotHostBuilder builder)
    {
        builder.Services.AddScoped<IPluginCollection, LambotPluginCollection>();
        var entryAssembly = Assembly.GetEntryAssembly();
        var directory = Path.GetDirectoryName(entryAssembly.Location);
        foreach (var file in Directory.GetFiles(directory, "*.dll"))
        {
            var assembly = Assembly.LoadFrom(file);
            var plugins = assembly.GetExportedTypes()
                 .Where(x => x.IsAssignableTo(typeof(LambotPlugin)))
                 .Where(x => x.IsClass && !x.IsAbstract)
                 .ToList();
            foreach (var plugin in plugins)
            {
                builder.Services.AddScoped(plugin);
                foreach (var pluginMethod in plugin.GetMethods())
                {
                    LambotPluginCollection.TryAdd(plugin, pluginMethod);
                }
            }
        }
    }
}
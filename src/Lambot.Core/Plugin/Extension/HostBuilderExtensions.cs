using Lambot.Core.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Lambot.Core;

public static class HostBuilderExtensions
{
    public static void RegisterPlugins(this HostBuilder builder)
    {
        builder.Services.AddScoped<IPluginCollection, PluginCollection>();
        var entryAssembly = Assembly.GetEntryAssembly();
        var directory = Path.GetDirectoryName(entryAssembly.Location);
        foreach (var file in Directory.GetFiles(directory, "*.dll"))
        {
            var fileName = Path.GetFileName(file);
            if (fileName.StartsWith("System.")) continue;
            if (fileName.StartsWith("Websocket.")) continue;
            if (fileName.StartsWith("Microsoft.")) continue;
            if (fileName.StartsWith("Newtonsoft.")) continue;
            if (fileName.StartsWith("Lambot.Core.")) continue;
            if (fileName.StartsWith("Lambot.Adapters.")) continue;

            var assembly = Assembly.LoadFrom(file);
            var plugins = assembly.GetTypes()
                 .Where(x => x.IsClass && !x.IsAbstract)
                 .Where(x => x.IsAssignableTo(typeof(PluginBase)))
                 .ToList();
            foreach (var plugin in plugins) 
            {
                builder.Services.AddScoped(plugin);
                var pluginAttr = plugin.GetCustomAttribute<PluginInfo>() ?? new PluginInfo
                {
                    Name = plugin.FullName,
                };
                foreach (var pluginMethod in plugin.GetMethods())
                {
                    PluginCollection.TryAdd(plugin, pluginAttr, pluginMethod);
                }
                Console.WriteLine($"Loading Plugin[{pluginAttr.Name} {pluginAttr.Version}]");
            }
        }
    }
}
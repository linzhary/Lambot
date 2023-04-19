using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core.Plugin;

public static class ServiceCollectionExtensions
{
    
    public static void RegisterPlugins(this IServiceCollection services)
    {
        services.AddSingleton<IPluginCollection, PluginCollection>();
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null) return;
        var directory = Path.GetDirectoryName(entryAssembly.Location);
        if(directory is null) return;
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
                services.AddScoped(plugin);
                var pluginAttr = plugin.GetCustomAttribute<PluginInfo>() ?? new PluginInfo
                {
                    Name = plugin.FullName,
                };
                foreach (var pluginMethod in plugin.GetMethods())
                {
                    PluginCollection.TryAdd(pluginAttr, pluginMethod);
                }
                Console.WriteLine($"Loading Plugin[{pluginAttr.Name} {pluginAttr.Version}]");
            }
        }
    }
}
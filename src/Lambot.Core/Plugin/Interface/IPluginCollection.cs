namespace Lambot.Core.Plugin;

public interface IPluginCollection
{
    Task OnMessageAsync(string serviceId, LambotEvent evt);
}
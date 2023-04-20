namespace Lambot.Core.Plugin;

public interface IPluginCollection
{
    Task OnMessageAsync(long client_id, LambotEvent evt);
}
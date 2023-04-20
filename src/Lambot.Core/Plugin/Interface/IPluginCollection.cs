namespace Lambot.Core.Plugin;

public interface IPluginCollection
{
    Task OnReceiveAsync(long sessionId, LambotEvent evt);
}
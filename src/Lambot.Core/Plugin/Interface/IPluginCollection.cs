namespace Lambot.Core.Plugin;

public interface IPluginCollection
{
    Task OnMessageAsync(long sessionId, LambotEvent evt);
}
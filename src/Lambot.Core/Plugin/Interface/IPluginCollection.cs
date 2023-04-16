namespace Lambot.Core;

public interface IPluginCollection
{
    void OnMessageAsync(LambotEvent evt);
}
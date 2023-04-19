namespace Lambot.Core.Plugin;

public interface IPluginCollection
{
    Task OnMessageAsync(LambotEvent evt);
}
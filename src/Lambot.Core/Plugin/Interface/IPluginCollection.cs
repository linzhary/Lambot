namespace Lambot.Core;

public interface IPluginCollection
{
    Task OnMessageAsync(LambotEvent evt);
}
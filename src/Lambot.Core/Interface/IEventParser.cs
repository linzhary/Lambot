namespace Lambot.Core;

public interface IEventParser
{
    LambotEvent Parse(string postMessage);
}
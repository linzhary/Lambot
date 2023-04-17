namespace Lambot.Core;

/// <summary>
/// 以Singleton模式注入
/// </summary>
public interface IEventParser
{
    LambotEvent Parse(string postMessage);
}
namespace Lambot.Core.Adapter;

/// <summary>
/// 以Singleton模式注入
/// </summary>
public interface IEventParser
{
    LambotEvent Parse(string postMessage);
}
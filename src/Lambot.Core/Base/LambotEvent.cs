namespace Lambot.Core;

public abstract class LambotEvent
{
    public long MessageId { get; set; }
    public string RawMessage { get; set; }
}
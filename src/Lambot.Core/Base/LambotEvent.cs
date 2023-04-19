namespace Lambot.Core;

public abstract class LambotEvent
{

    public string RequestId { get; set; }
    public long MessageId { get; set; }
    public string RawMessage { get; set; }
}
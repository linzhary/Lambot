namespace Lambot.Adapters.OneBot;

public class AtMessageSeg : MessageSeg
{
    public override MessageSegType Type => MessageSegType.At;
    public long UserId { get; set; } = 0;

    public AtMessageSeg()
    {
    }

    public AtMessageSeg(Dictionary<string, string?> props)
    {
        UserId = long.Parse(props.GetValueOrDefault("qq", "0")!);
    }

    protected override Dictionary<string, string?> GetProps()
    {
        return new()
        {
            { "qq", UserId.ToString() }
        };
    }
}
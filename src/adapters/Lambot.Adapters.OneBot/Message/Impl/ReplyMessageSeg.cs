namespace Lambot.Adapters.OneBot;

public class ReplyMessageSeg : MessageSeg
{
    public ReplyMessageSeg(Dictionary<string, string?> props)
    {
        Id = props.GetValueOrDefault("id")!;
    }

    public override MessageSegType Type => MessageSegType.Image;
    public string Id { get; set; }
    protected override Dictionary<string, string?> GetProps()
    {
        return new()
        {
            { "id", Id },
        };
    }
}
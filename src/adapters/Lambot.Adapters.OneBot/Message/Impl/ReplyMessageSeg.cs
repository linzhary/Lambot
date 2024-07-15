namespace Lambot.Adapters.OneBot;

public class ReplyMessageSeg : MessageSeg
{
    public ReplyMessageSeg(long id)
    {
        Id = id.ToString();
    }
    public ReplyMessageSeg(Dictionary<string, string?> props)
    {
        Id = props.GetValueOrDefault("id")!;
    }

    public override MessageSegType Type => MessageSegType.Reply;
    public string Id { get; set; }
    protected override Dictionary<string, string?> GetProps()
    {
        return new()
        {
            { "id", Id },
        };
    }
}
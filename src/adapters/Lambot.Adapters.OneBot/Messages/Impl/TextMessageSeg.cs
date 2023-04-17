namespace Lambot.Adapters.OneBot;

public class TextMessageSeg : MessageSeg
{
    public override MessageSegType Type => MessageSegType.Text;
    public string Text { get; set; }

    protected override Dictionary<string, string> GetProps()
    {
        throw new NotImplementedException();
    }
}
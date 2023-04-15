namespace Lambot.Adapters.OneBot;
public class MessageSegment
{
    public string Type { get; private set; }
    public Dictionary<string, object> Props { get; private set; }
    public override string ToString()
    {
        if (Type == "text")
        {
            return Props["text"].ToString();
        }
        var props = string.Concat(Props.Select(x => $",{x.Key}={x.Value}"));
        return $"[CQ:{Type.ToLower()}{props}]";

    }
    public MessageSegment(string type)
    {
        Type = type;
        Props = new Dictionary<string, object>();
    }
    public static MessageSegment At(int userId)
    {
        var seg = new MessageSegment("at");
        seg.Props.Add("qq", userId);
        return seg;
    }
    public static MessageSegment Text(string text)
    {
        var seg = new MessageSegment("text");
        seg.Props.Add("text", text);
        return seg;
    }
}
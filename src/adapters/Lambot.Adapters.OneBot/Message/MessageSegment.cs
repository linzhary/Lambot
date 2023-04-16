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

    public MessageSegment(string type, Dictionary<string, object> props = null)
    {
        Type = type;
        Props = props ?? new Dictionary<string, object>();
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

    internal static MessageSegment Parse(string code_seg)
    {
        var sections = code_seg[4..^1].Split(',');
        var seg = new MessageSegment(sections[0]);
        foreach (var section in sections[1..])
        {
            var kv = section.Split('=');
            if (kv.Length < 2) continue;
            else if (kv.Length == 2)
            {
                seg.Props[kv[0]] = kv[1];
            }
            else
            {
                seg.Props[kv[0]] = string.Join('=', kv[1..]);
            }
        }
        return seg;
    }
}
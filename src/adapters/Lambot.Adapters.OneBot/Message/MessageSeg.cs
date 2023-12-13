using Lambot.Core;

namespace Lambot.Adapters.OneBot;

public abstract class MessageSeg
{
    public abstract MessageSegType Type { get; }

    protected abstract Dictionary<string, string?> GetProps();

    public override string ToString()
    {
        if (this is TextMessageSeg seg)
        {
            return seg.Text;
        }
        var type_str = Type.ToString().ToSnakeCase();
        var prop_str = string.Join(',', GetProps().Where(x => x.Value is not null).Select(x => $"{x.Key}={x.Value}"));
        return $"[CQ:{type_str},{prop_str}]";
    }

    public static MessageSeg Parse(string code_seg)
    {
        var sections = code_seg[4..^1].Split(',');
        if (!Enum.TryParse(sections[0].ToPascalCase(), true, out MessageSegType type))
        {
            return new TextMessageSeg(code_seg);
        }

        var props = new Dictionary<string, string?>();
        foreach (var section in sections[1..])
        {
            var kv = section.Split('=');
            if (kv.Length < 2) continue;
            else if (kv.Length == 2)
            {
                props[kv[0]] = kv[1];
            }
            else
            {
                props[kv[0]] = string.Join('=', kv[1..]);
            }
        }
        return type switch
        {
            MessageSegType.At => new AtMessageSeg(props),
            MessageSegType.Image => new ImageMessageSeg(props),
            MessageSegType.Reply => new ReplyMessageSeg(props),
            _ => throw new NotImplementedException(),
        };
    }
}
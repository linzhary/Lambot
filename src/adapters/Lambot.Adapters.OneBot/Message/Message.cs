using Newtonsoft.Json.Linq;

namespace Lambot.Adapters.OneBot;

public class Message
{
    public List<MessageSeg> Segments { get; private set; } = new List<MessageSeg>();
    public bool IsEmpty => Segments.Count == 0;

    public static explicit operator Message(string raw_message)
    {
        var message = new Message();
        if (string.IsNullOrWhiteSpace(raw_message))
        {
            return message;
        }

        var parser = new StringParser(raw_message);
        while (!parser.IsEnd)
        {
            if (parser.Current == '[')
            {
                var code_seg = parser.ReadTo(']', true);
                message.Segments.Add(MessageSeg.Parse(code_seg));
            }
            else
            {
                message.Segments.Add(new TextMessageSeg(parser.ReadTo('[')));
            }
        }
        return message;
    }

    public static explicit operator string(Message message)
    {
        return message.ToString();
    }

    public override string ToString()
    {
        return string.Concat(Segments.Select(x => x.ToString()));
    }
}
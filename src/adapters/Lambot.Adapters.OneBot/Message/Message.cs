namespace Lambot.Adapters.OneBot;

public class Message
{
    public List<MessageSegment> Segments { get; private set; } = new List<MessageSegment>();

    public static Message Parse(string raw_message)
    {
        var message = new Message();

        var parser = new StringParser(raw_message);
        while (!parser.IsEnd)
        {
            if (parser.Current == '[')
            {
                var code_seg = parser.ReadTo(']', true);
                message.Segments.Add(MessageSegment.Parse(code_seg));
            }
            else
            {
                var text = parser.ReadTo('[');
                message.Segments.Add(MessageSegment.Text(text));
            }
        }
        return message;
    }

    public override string ToString()
    {
        return string.Concat(Segments.Select(x => x.ToString()));
    }
}
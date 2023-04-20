namespace Lambot.Adapters.OneBot;

public class Message
{
    public List<MessageSeg> Segments { get; private set; } = new List<MessageSeg>();

    public static Message Parse(string raw_message)
    {
        var message = new Message();

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
                message.Segments.Add(new TextMessageSeg
                {
                    Text = parser.ReadTo('[')
                });
            }
        }
        return message;
    }

    public override string ToString()
    {
        return string.Concat(Segments.Select(x => x.ToString()));
    }
}
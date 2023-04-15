using Lambot.Core;
using System.Reactive.Joins;
using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot;


public class Message
{
    public string RawMessage => string.Concat(Segments.Select(x => x.ToString()));

    public List<MessageSegment> Segments { get; private set; } = new List<MessageSegment>();

    public static Message Parse(string raw_message)
    {
        var match = Regex.Match(raw_message, @"\[CQ:\S+,([\s]\S+=\S+[\s])+\]");

        return new Message();
    }
}

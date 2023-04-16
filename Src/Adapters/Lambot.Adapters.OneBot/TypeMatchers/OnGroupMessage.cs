namespace Lambot.Adapters.OneBot;

public class OnGroupMessage : OnMessage
{
    public OnGroupMessage()
    {
        Priority -= 1;
    }

    public override int Type => (int)Matcher.Type.OnGroupMessage;
}
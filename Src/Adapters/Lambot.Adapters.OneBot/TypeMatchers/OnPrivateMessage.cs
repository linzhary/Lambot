namespace Lambot.Adapters.OneBot.TypeMatchers;

public class OnPrivateMessage : OnMessage
{
    public OnPrivateMessage()
    {
        Priority -= 2;
    }
    public override int Type => (int)Matcher.Type.OnGroupMessage;
}

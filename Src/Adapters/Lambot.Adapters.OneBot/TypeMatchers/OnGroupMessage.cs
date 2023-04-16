namespace Lambot.Adapters.OneBot;

public class OnGroupMessage : OnMessage
{
    public OnGroupMessage()
    {
        Priority -= 1;
    }
}
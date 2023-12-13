namespace Lambot.Adapters.OneBot;

public class OnGroupMessage : OnMessage
{
    public OnGroupMessage()
    {
        Type = MessageType.Group;
    }
}
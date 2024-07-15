namespace Lambot.Adapters.OneBot;

public class OnPrivateMesssage : OnMessage
{
    public OnPrivateMesssage()
    {
        Type = MessageType.Private;
    }
}
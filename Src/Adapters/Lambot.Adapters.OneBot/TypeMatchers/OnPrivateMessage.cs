namespace Lambot.Adapters.OneBot;

public class OnPrivateMessage : OnMessage
{
    public OnPrivateMessage()
    {
        Priority -= 2;
    }
}
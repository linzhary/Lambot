using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnMessage : TypeMatcher
{
    public override int Type => (int)Matcher.Type.OnMessage;
}
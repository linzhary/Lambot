using Lambot.Plugin;

namespace Lambot.Adapters.OneBot.TypeMatchers;

public class OnMessage : TypeMatcherAttribute
{
    public override int Type => (int)Matcher.Type.OnMessage;
}

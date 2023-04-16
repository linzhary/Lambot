using Lambot.Core.Plugin;

namespace Lambot.Adapters.OneBot;

public class OnMessage : TypeMatcherAttribute
{
    public override int Type => (int)Matcher.Type.OnMessage;
}
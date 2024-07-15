using Lambot.Core;
using Lambot.Core.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class CommandArgs
{

}
public class OnCommand : RuleMatcher
{
    private readonly string _text;

    public OnCommand(string text)
    {
        _text = $"/{text}";
        if (Priority == int.MaxValue)
        {
            Priority = int.MaxValue - 1;
        }
    }

    public string GetCommand()
    {
        return _text;
    }

    public override bool Check(LambotEvent evt)
    {
        if (evt is not BaseMessageEvent messageEvent) return false;
        if (string.IsNullOrWhiteSpace(messageEvent.RawMessage)) return false;
        return messageEvent.RawMessage.StartsWith(_text);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class AtMessageSeg : MessageSeg
{
    public override MessageSegType Type => MessageSegType.At;
    public string UserId { get; set; }
    public AtMessageSeg()
    {
    }
    public AtMessageSeg(Dictionary<string, string> props)
    {
        UserId = props.GetValueOrDefault("qq");
    }
    protected override Dictionary<string, string> GetProps()
    {
        return new()
        {
            { "qq", UserId }
        };
    }
}

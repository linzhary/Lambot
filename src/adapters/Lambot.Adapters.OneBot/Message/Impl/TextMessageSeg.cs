using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public class TextMessageSeg : MessageSeg
{
    public override MessageSegType Type => MessageSegType.Text;
    public new string Text { get; set; }
    protected override Dictionary<string, string> GetProps()
    {
        throw new NotImplementedException();
    }
}

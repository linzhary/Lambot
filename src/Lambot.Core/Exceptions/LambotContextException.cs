using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Core.Exceptions;

public class LambotContextException : Exception
{
    public bool IsSkip { get;  set; } = false;
    public bool IsBreak { get;  set; } = false;
    public bool IsFinish { get;  set; } = false;
    public LambotContextException(string message) : base(message)
    {
    }
}
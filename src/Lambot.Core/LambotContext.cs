using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using Lambot.Core.Exceptions;

namespace Lambot.Core;

public class LambotContext
{
    public long ClientId { get; set; } = -1;

    public bool IsBreaked { get; set; }

    public Exception Skip()
    {
        IsBreaked = false;
        return new LambotContextException("skip")
        {
            IsSkip = true
        };
    }

    public Exception Break()
    {
        IsBreaked = true;
        return new LambotContextException("break")
        {
            IsBreak = true
        };
    }

    public Exception Finish(string message, bool? @break = null)
    {
        if (@break.HasValue)
        {
            IsBreaked = @break.Value;
        }

        return new LambotContextException(message)
        {
            IsFinish = true
        };
    }
}
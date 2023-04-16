using Lambot.Core;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Lambot.Adapters.OneBot;

internal class OneBotEventMatcher : IPluginMatcher
{
    private readonly ILogger<OneBotEventMatcher> _logger;
    private readonly LambotContext _context;

    public OneBotEventMatcher(ILogger<OneBotEventMatcher> logger, LambotContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void Invoke(PluginMatcherParameter parameter)
    {
        try
        {
            switch ((Matcher.Type)parameter.MatchType)
            {
                case Matcher.Type.OnMessage:
                    Invoke<MessageEvent>(_context, parameter);
                    break;

                case Matcher.Type.OnGroupMessage:
                    Invoke<GroupMessageEvent>(_context, parameter);
                    break;

                case Matcher.Type.OnPrivateMessage:
                    Invoke<PrivateMessageEvent>(_context, parameter);
                    break;

                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Invoke Matcher Fail : {msg}", parameter.Event.RawMessage);
        }
    }

    private static void Invoke<TEvent>(LambotContext context, PluginMatcherParameter parameter)
        where TEvent : LambotEvent
    {
        if (parameter.Event is not TEvent) return;
        if (!parameter.IsRuleMatched) return;

        context.IsBreak = parameter.TypeMatcher.Break;

        var instanceExpr = Expression.Constant(parameter.PluginInstance, parameter.PluginInfo.Type);
        var parameterExpr = Expression.Parameter(typeof(TEvent));
        var callExpr = Expression.Call(instanceExpr, parameter.PluginInfo.MethodName, null, parameterExpr);
        var lambdaExpr = Expression.Lambda<Action<TEvent>>(callExpr, parameterExpr);
        lambdaExpr.Compile().Invoke(parameter.Event as TEvent);
    }
}
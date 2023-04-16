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
                    Invoke<MessageEvent>(parameter);
                    break;

                case Matcher.Type.OnGroupMessage:
                    Invoke<GroupMessageEvent>(parameter);
                    break;

                case Matcher.Type.OnPrivateMessage:
                    Invoke<PrivateMessageEvent>(parameter);
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

    private void Invoke<TEvent>(PluginMatcherParameter parameter)
        where TEvent : LambotEvent
    {
        if (parameter.Event is not TEvent) return;
        if (!parameter.IsRuleMatched) return;

        _logger.LogInformation("消息 [{message_id}] 匹配到 [{plugin__name}] 的 [{method_name}]"
            , parameter.Event.MessageId, parameter.PluginInfo.Name, parameter.PluginTypeInfo.MethodName);

        _context.IsBreak = parameter.TypeMatcher.Break;

        var instanceExpr = Expression.Constant(parameter.PluginInstance, parameter.PluginTypeInfo.Type);
        var parameterExpr = Expression.Parameter(typeof(TEvent));
        var callExpr = Expression.Call(instanceExpr, parameter.PluginTypeInfo.MethodName, null, parameterExpr);
        var lambdaExpr = Expression.Lambda<Action<TEvent>>(callExpr, parameterExpr);
        lambdaExpr.Compile().Invoke(parameter.Event as TEvent);
    }
}
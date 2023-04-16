using Lambot.Core;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot;

internal class OneBotEventMatcher : IPluginMatcher
{
    private readonly ILogger<OneBotEventMatcher> _logger;
    private readonly LambotContext _context;
    private readonly Bot _bot;

    public OneBotEventMatcher(ILogger<OneBotEventMatcher> logger, LambotContext context, Bot bot)
    {
        _logger = logger;
        _context = context;
        _bot = bot;
    }

    public void Invoke(PluginMatcherParameter parameter)
    {
        if (parameter.TypeMatcher is OnGroupMessage)
        {
            Invoke<GroupMessageEvent>(parameter);
        }
        else if (parameter.TypeMatcher is OnPrivateMessage)
        {
            Invoke<PrivateMessageEvent>(parameter);
        }
        else if (parameter.TypeMatcher is OnMessage)
        {
            Invoke<MessageEvent>(parameter);
        }
    }

    private void Invoke<TEvent>(PluginMatcherParameter parameter)
        where TEvent : LambotEvent
    {
        if (parameter.Event is not TEvent) return;
        if (!parameter.IsRuleMatched) return;

        _logger.LogInformation("消息 [{message_id}] 匹配到 [{plugin__name}] 的 [{method_name}]"
            , parameter.Event.MessageId, parameter.PluginInfo.Name, parameter.MethodInfo.Name);

        _context.IsBreak = parameter.TypeMatcher.Break;

        var parameters = new List<object>();
        foreach (var parameterInfo in parameter.MethodInfo.GetParameters())
        {
            if (parameterInfo.ParameterType.IsAssignableTo(typeof(Bot)))
            {
                parameters.Add(_bot);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(LambotContext)))
            {
                parameters.Add(_context);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(TEvent)))
            {
                parameters.Add(parameter.Event);
            }
            else if (parameterInfo.ParameterType.IsValueType)
            {
                parameters.Add(Activator.CreateInstance(parameterInfo.ParameterType));
            }
            else if (parameter.RuleMatcher is OnRegex regexMatcher)
            {
                if (parameterInfo.ParameterType.IsAssignableTo(typeof(Match)))
                {
                    parameters.Add(regexMatcher.MatchResult);
                }
                else if (parameterInfo.ParameterType.IsAssignableTo(typeof(Group[])))
                {
                    parameters.Add(regexMatcher.MatchResult.Groups.Cast<Group>().Skip(1).ToArray());
                }
                else if (parameterInfo.ParameterType.IsAssignableTo(typeof(IEnumerable<Group>)))
                {
                    parameters.Add(regexMatcher.MatchResult.Groups.Cast<Group>().Skip(1));
                }
                else
                {
                    parameters.Add(null);
                }
            }
            else
            {
                parameters.Add(null);
            }
        }
        parameter.MethodInfo.Invoke(parameter.PluginInstance, parameters.ToArray());
    }
}
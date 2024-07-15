using Lambot.Core;
using Lambot.Core.Exceptions;
using Lambot.Core.Plugin;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot;

internal class OneBotEventMatcher : IPluginMatcher
{
    private readonly ILogger<OneBotEventMatcher> _logger;
    private readonly LambotContext _context;
    private readonly OneBotClient _oneBotClient;

    public OneBotEventMatcher(ILogger<OneBotEventMatcher> logger, LambotContext context, OneBotClient oneBotClient)
    {
        _logger = logger;
        _context = context;
        _oneBotClient = oneBotClient;
    }

    public async Task InvokeAsync(PluginMatcherParameter parameter)
    {
        if (parameter.Event is GroupMessageEvent groupMessageEvent && _oneBotClient.ProcessWaitGroupMessage(groupMessageEvent)) return;

        var result = await DyamicInvokeAsync(parameter);
        if (result is string raw_message && !string.IsNullOrEmpty(raw_message))
        {
            if (parameter.Event is IGroupEvent groupEvt)
            {
                await _oneBotClient.SendGroupMessageAsync(groupEvt.GroupId, raw_message);
            }
            else if (parameter.Event is IPrivateEvent privateEvt)
            {
                await _oneBotClient.SendPrivateMessageAsync(privateEvt.UserId, raw_message, privateEvt.GroupId);
            }
        }
        else if (result is Message message)
        {
            if (parameter.Event is IGroupEvent groupEvt)
            {
                await _oneBotClient.SendGroupMessageAsync(groupEvt.GroupId, message);
            }
            else if (parameter.Event is IPrivateEvent privateEvt)
            {
                await _oneBotClient.SendPrivateMessageAsync(privateEvt.UserId, message, privateEvt.GroupId);
            }
        }
    }

    private readonly ConcurrentDictionary<MethodInfo, Delegate> _methodCache = new();

    private async Task<object?> DyamicInvokeAsync(PluginMatcherParameter parameter)
    {
        if (!parameter.IsTypeChecked) return null;
        if (!parameter.IsRuleChecked) return null;
        if (!parameter.IsPermChecked) return null;

        if (parameter.Event is BaseMessageEvent messageEvent)
        {
            _logger.LogInformation("消息 [{message_id}] 匹配到 [{plugin__name}] 的 [{method_name}]"
            , messageEvent.MessageId, parameter.PluginInfo.Name, parameter.MethodInfo.Name);
            if (parameter.RuleMatcher is OnCommand commandMatcher)
            {
                messageEvent.RawMessage = messageEvent.RawMessage.Replace($"{commandMatcher.GetCommand()} ", string.Empty);
                messageEvent.Convert();
            }
        }
        //if (parameter.Event is BaseNoticeEvent)
        //{

        //}
        _context.IsBreaked = parameter.TypeMatcher!.Break;

        var parameterValues = new List<object?>();
        var parameterInfos = parameter.MethodInfo.GetParameters();
        foreach (var parameterInfo in parameterInfos)
        {
            if (parameterInfo.ParameterType.IsAssignableTo(typeof(OneBotClient)))
            {
                parameterValues.Add(_oneBotClient);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(LambotContext)))
            {
                parameterValues.Add(_context);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(LambotEvent)))
            {
                parameterValues.Add(parameter.Event);
            }
            else if (parameterInfo.ParameterType.IsValueType)
            {
                parameterValues.Add(Activator.CreateInstance(parameterInfo.ParameterType));
            }
            else if (parameter.RuleMatcher is OnRegex regexMatcher)
            {
                if (regexMatcher.MatchResult is null)
                {
                    parameterValues.Add(null);
                }
                else if (parameterInfo.ParameterType.IsAssignableTo(typeof(Match)))
                {
                    parameterValues.Add(regexMatcher.MatchResult);
                }
                else if (parameterInfo.ParameterType.IsAssignableTo(typeof(Group[])))
                {
                    parameterValues.Add(regexMatcher.MatchResult.Groups.Cast<Group>().Skip(1).ToArray());
                }
                else if (parameterInfo.ParameterType.IsAssignableTo(typeof(IEnumerable<Group>)))
                {
                    parameterValues.Add(regexMatcher.MatchResult.Groups.Cast<Group>().Skip(1));
                }
                else
                {
                    parameterValues.Add(null);
                }
            }
            else
            {
                parameterValues.Add(null);
            }
        }
        try
        {
            return await _methodCache.GetOrAdd(parameter.MethodInfo, (_) =>
            {
                var instanceExpr = Expression.Parameter(parameter.MethodInfo.DeclaringType!, "instance");
                var parameterExprs = new List<ParameterExpression>();
                for (var i = 0; i < parameterInfos.Count(); i++)
                {
                    parameterExprs.Add(Expression.Parameter(parameterInfos[i].ParameterType, $"arg_{i}"));
                }
                var methodCallExpr = Expression.Call(instanceExpr, parameter.MethodInfo, parameterExprs);
                return Expression.Lambda(methodCallExpr, new[] { instanceExpr }.Concat(parameterExprs).ToArray()).Compile();
            })
            .DynamicInvoke(new[] { parameter.PluginInstance }.Concat(parameterValues).ToArray())
            .UnwrapAsyncResult();
        }
        catch (LambotContextException ex)
        {
            if (ex.IsFinish)
            {
                return ex.Message;
            }
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException is LambotContextException innerEx)
            {
                if (innerEx.IsFinish)
                {
                    return innerEx.Message;
                }
            }
        }
        catch(Exception ex)
        {
            return ex.Message;
        }
        return null;
    }
}
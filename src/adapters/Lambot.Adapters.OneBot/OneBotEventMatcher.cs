﻿using Lambot.Core;
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
    private readonly Bot _bot;

    public OneBotEventMatcher(ILogger<OneBotEventMatcher> logger, LambotContext context, Bot bot)
    {
        _logger = logger;
        _context = context;
        _bot = bot;
    }

    public async Task InvokeAsync(PluginMatcherParameter parameter)
    {
        object result = null;
        if (parameter.TypeMatcher is OnGroupMessage)
        {
            result = await Invoke<GroupMessageEvent>(parameter);
        }
        else if (parameter.TypeMatcher is OnPrivateMessage)
        {
            result = await Invoke<PrivateMessageEvent>(parameter);
        }
        else if (parameter.TypeMatcher is OnMessage)
        {
            result = await Invoke<MessageEvent>(parameter);
        }
        if (result is string raw_message && !string.IsNullOrEmpty(raw_message))
        {
            if (parameter.Event is GroupMessageEvent groupEvt)
            {
                await _bot.SendGroupMessageAsync(groupEvt.GroupId, Message.Parse(raw_message));
            }
            else if (parameter.Event is PrivateMessageEvent privateEvt)
            {
                await _bot.SendPrivateMessageAsync(privateEvt.UserId, Message.Parse(raw_message), privateEvt.GroupId);
            }
        }
        else if (result is Message message && message is not null)
        {
            if (parameter.Event is GroupMessageEvent groupEvt)
            {
                await _bot.SendGroupMessageAsync(groupEvt.GroupId, message);
            }
            else if (parameter.Event is PrivateMessageEvent privateEvt)
            {
                await _bot.SendPrivateMessageAsync(privateEvt.UserId, message, privateEvt.GroupId);
            }
        }
    }

    private readonly ConcurrentDictionary<MethodInfo, Delegate> _methodCache = new();
    private async Task<object> Invoke<TEvent>(PluginMatcherParameter parameter)
        where TEvent : LambotEvent
    {
        if (parameter.Event is not TEvent) return null;
        if (!parameter.IsRuleMatched) return null;
        _logger.LogInformation("消息 [{message_id}] 匹配到 [{plugin__name}] 的 [{method_name}]"
            , parameter.Event.MessageId, parameter.PluginInfo.Name, parameter.MethodInfo.Name);

        _context.IsBreaked = parameter.TypeMatcher.Break;

        var parameterValues = new List<object>();
        var parameterInfos = parameter.MethodInfo.GetParameters();
        foreach (var parameterInfo in parameterInfos)
        {
            if (parameterInfo.ParameterType.IsAssignableTo(typeof(Bot)))
            {
                parameterValues.Add(_bot);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(LambotContext)))
            {
                parameterValues.Add(_context);
            }
            else if (parameterInfo.ParameterType.IsAssignableTo(typeof(TEvent)))
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
                var instanceExpr = Expression.Parameter(parameter.MethodInfo.DeclaringType, "instance");
                var parameterExprs = new List<ParameterExpression>();
                for (var i = 0; i < parameter.MethodInfo.GetParameters().Count(); i++)
                {
                    parameterExprs.Add(Expression.Parameter(parameter.MethodInfo.GetParameters()[i].ParameterType, $"arg_{i}"));
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
                    return ex.Message;
                }
            }
        }
        return null;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

public static class MessageSubType
{
    /// <summary>
    /// 好友消息
    /// </summary>
    public const string Friend = "friend";
    /// <summary>
    /// 群聊消息
    /// </summary>
    public const string Normal = "normal";
    /// <summary>
    /// 匿名消息
    /// </summary>
    public const string Anonymous = "anonymous";
    /// <summary>
    /// 自己的群聊消息
    /// </summary>
    public const string GroupSelf = "group_self";
    /// <summary>
    /// 群临时会话
    /// </summary>
    public const string Group = "group";
    /// <summary>
    /// 系统提示
    /// </summary>
    public const string Notice = "notice";

    public static string GetChinese(string sub_type)
    {
        return sub_type switch
        {
            Friend => "好友消息",
            Normal => "群聊消息",
            Anonymous => "匿名消息",
            GroupSelf => "本人群聊消息",
            Group => "临时会话",
            Notice => "系统提示",
            _ => "未知消息",
        };
    }
}

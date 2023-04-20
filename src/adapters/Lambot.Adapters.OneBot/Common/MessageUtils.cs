namespace Lambot.Adapters.OneBot;

internal static class MessageUtils
{
    public static object? ConvertValue(object? obj, Type type) 
    {
        if (obj is null) return default;
        if (obj is not string) return default;
        if (!type.IsEnum) return default;
        var str = ((string)obj).Trim().ToPascalCase();
        var names = Enum.GetNames(type);
        if (names.Contains(str))
        {
            return Enum.Parse(type, str);
        }
        else
        {
            return default;
        }
    }

    public static T ConvertTo<T>(object? obj) where T : struct, Enum
    {
        if (obj is null) return default;
        if (obj is not string) return default;
        var names = Enum.GetNames<T>();
        var str = ((string)obj).Trim().ToPascalCase();
        if (names.Contains(str))
        {
            return Enum.Parse<T>(str);
        }
        else
        {
            return default;
        }
    }

    public static string GetChinese(MessageSubType sub_type)
    {
        return sub_type switch
        {
            MessageSubType.Friend => "好友消息",
            MessageSubType.Normal => "群聊消息",
            MessageSubType.Anonymous => "匿名消息",
            MessageSubType.GroupSelf => "本人群聊消息",
            MessageSubType.Group => "临时会话",
            MessageSubType.Notice => "系统提示",
            _ => "未知消息",
        };
    }
}
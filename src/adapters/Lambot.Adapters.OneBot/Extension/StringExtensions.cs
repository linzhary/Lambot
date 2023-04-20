using System.Text.RegularExpressions;

namespace Lambot.Adapters.OneBot;

internal static class StringExtensions
{
    /// <summary>
    /// SnakeCase To PascalCase
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return Regex.Replace('_' + str, "_[a-z]", m => m.Value.ToUpper().Trim('_'));
    }

    /// <summary>
    /// PascalCase To SnakeCase
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return Regex.Replace(str, "[A-Z0-9][0-9]*", "_$0").ToLower().Trim('_');
    }
}
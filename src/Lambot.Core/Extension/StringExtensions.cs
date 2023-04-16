using System.Text;
using System.Text.RegularExpressions;

namespace Lambot.Core;

public static class StringExtensions
{
    /// <summary>
    /// SnakeCase To PascalCase
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToPascalCase(this string str)
        => Regex.Replace(str, "_[a-z0-9]", m => m.Value.ToUpper().Trim('_'));
    /// <summary>
    /// PascalCase To SnakeCase
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
        => Regex.Replace(str, "[A-Z0-9][0-9]*", "_$0").ToLower().Trim('_');
}
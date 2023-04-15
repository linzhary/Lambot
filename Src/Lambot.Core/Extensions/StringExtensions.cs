using System.Text;

namespace Lambot.Core;

public static class StringExtensions
{
    public static string ToPascalCase(this string source)
    {
        // 如果字符串为空或空白，直接返回
        if (string.IsNullOrWhiteSpace(source))
        {
            return source;
        }

        // 将字符串按下划线分隔
        var parts = source.Split('_');

        // 创建一个StringBuilder对象，用于拼接结果
        var result = new StringBuilder();

        // 遍历每个单词
        foreach (var part in parts)
        {
            // 如果单词为空或空白，跳过
            if (string.IsNullOrWhiteSpace(part))
            {
                continue;
            }

            // 将单词的首字母大写，其余小写，然后追加到结果中
            result.Append(char.ToUpper(part[0]) + part[1..].ToLower());
        }

        // 返回结果字符串
        return result.ToString();
    }
}

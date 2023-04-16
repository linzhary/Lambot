namespace Lambot.Core;

public static class EnumableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> predicate)
    {
        if (predicate is null) return;
        foreach (var item in source)
        {
            predicate(item);
        }
    }
}
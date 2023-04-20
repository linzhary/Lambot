namespace Lambot.Core.Adapter;

internal static class AdapterCollection
{
    internal static readonly Dictionary<Type, IAdapter> Adapters = new();
}
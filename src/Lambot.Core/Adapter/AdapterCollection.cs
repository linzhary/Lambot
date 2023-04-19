namespace Lambot.Core.Adapter;

internal static class AdapterCollection
{
    internal static readonly Dictionary<AdapterType, IAdapter> Adapters = new();
}
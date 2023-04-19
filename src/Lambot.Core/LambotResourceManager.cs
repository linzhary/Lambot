using System.Collections.Concurrent;

namespace Lambot.Core;

public class LambotResourceManager
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<string>> _receivedQueues = new();

    internal void RemoveReceivedQueue(string serviceId)
    {
        _receivedQueues.TryRemove(serviceId, out _);
    }

    internal ConcurrentQueue<string> GetOrAddReceivedQueue(string serviceId)
    {
        return _receivedQueues.GetOrAdd(serviceId, _ => new());
    }

    internal ICollection<ConcurrentQueue<string>> AllReceivedQueues => _receivedQueues.Values;
}
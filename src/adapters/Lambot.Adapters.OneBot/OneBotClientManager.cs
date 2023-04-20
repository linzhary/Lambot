using System.Collections.Concurrent;

namespace Lambot.Adapters.OneBot;

public class OneBotClientManager
{
    private readonly ConcurrentDictionary<long, OneBotClient> _clientMap = new();

    internal void Add(OneBotClient client)
    {
        _clientMap.AddOrUpdate(client.UserId, client, (_, _) => client);
    }

    internal OneBotClient? Get(long user_id)
    {
        return _clientMap!.GetValueOrDefault(user_id, null);
    }
}
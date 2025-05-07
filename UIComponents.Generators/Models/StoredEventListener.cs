using System.Collections.Concurrent;

namespace UIComponents.Generators.Models;

public class StoredEventListener
{
    public string Guid { get; set; }
    public Dictionary<string, bool> ConnectionIds { get; set; } = new();

    public Action SubscribeEvent { get; set; }
    public Action UnSubscribeEvent { get; set; }
    public DateTime SubscriptionChanged { get; private set; } = DateTime.Now;
    public bool Subscribed { get; private set; }

    public void AddConnectionId(string connectionId)
    {
        if (ConnectionIds.TryGetValue(connectionId, out bool connected))
        {
            if (connected)
                return;
        }
        ConnectionIds[connectionId] = true;

        if (!Subscribed)
        {
            SubscribeEvent();
            Subscribed = true;
            SubscriptionChanged = DateTime.Now;
        }
    }

    public void RemoveConnectionId(string connectionId)
    {
        ConnectionIds[connectionId] = false;
        if (!ConnectionIds.Where(x=>x.Value).Any())
        {
            UnSubscribeEvent();
            Subscribed = false;
            SubscriptionChanged = DateTime.Now;
        }
    }
}

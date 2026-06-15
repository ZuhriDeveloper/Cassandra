using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers.Events;

namespace Cassandra.Infrastructure.Persistence.EventStore;

public static class EventTypeRegistry
{
    private static readonly Dictionary<string, Type> Map = new()
    {
        // Dealer (platform / multi-tenant) events
        [nameof(DealerRegistered)]  = typeof(DealerRegistered),
        [nameof(DealerRenamed)]     = typeof(DealerRenamed),
        [nameof(DealerActivated)]   = typeof(DealerActivated),
        [nameof(DealerDeactivated)] = typeof(DealerDeactivated),
    };

    public static Type Resolve(string eventType) =>
        Map.TryGetValue(eventType, out var type)
            ? type
            : throw new InvalidOperationException($"Unknown event type: '{eventType}'.");

    public static string GetName(IDomainEvent evt) => evt.GetType().Name;
}

using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers.Events;
using Cassandra.Domain.Jabatan.Events;
using Cassandra.Domain.Karyawan.Events;

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

        // Jabatan (dealer-scoped) events
        [nameof(JabatanCreated)]    = typeof(JabatanCreated),
        [nameof(JabatanUpdated)]    = typeof(JabatanUpdated),
        [nameof(JabatanActivated)]  = typeof(JabatanActivated),
        [nameof(JabatanDeactivated)] = typeof(JabatanDeactivated),

        // Karyawan (dealer-scoped) events
        [nameof(KaryawanCreated)]     = typeof(KaryawanCreated),
        [nameof(KaryawanUpdated)]     = typeof(KaryawanUpdated),
        [nameof(KaryawanActivated)]   = typeof(KaryawanActivated),
        [nameof(KaryawanDeactivated)] = typeof(KaryawanDeactivated),
        [nameof(KaryawanResigned)]    = typeof(KaryawanResigned),
        [nameof(KaryawanLimitSet)]    = typeof(KaryawanLimitSet),
    };

    public static Type Resolve(string eventType) =>
        Map.TryGetValue(eventType, out var type)
            ? type
            : throw new InvalidOperationException($"Unknown event type: '{eventType}'.");

    public static string GetName(IDomainEvent evt) => evt.GetType().Name;
}

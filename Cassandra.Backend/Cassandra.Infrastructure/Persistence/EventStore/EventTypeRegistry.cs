using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers.Events;
using Cassandra.Domain.GrupTipeMotor.Events;
using Cassandra.Domain.Jabatan.Events;
using Cassandra.Domain.Karyawan.Events;
using Cassandra.Domain.Kelengkapan.Events;
using Cassandra.Domain.Kios.Events;
using Cassandra.Domain.Mediator.Events;
using Cassandra.Domain.TipeMotor.Events;
using Cassandra.Domain.Warna.Events;

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

        // Kios (dealer-scoped) events
        [nameof(KiosCreated)]         = typeof(KiosCreated),
        [nameof(KiosUpdated)]         = typeof(KiosUpdated),
        [nameof(KiosActivated)]       = typeof(KiosActivated),
        [nameof(KiosDeactivated)]     = typeof(KiosDeactivated),
        [nameof(KiosLimitSet)]        = typeof(KiosLimitSet),

        // Mediator (dealer-scoped) events
        [nameof(MediatorCreated)]     = typeof(MediatorCreated),
        [nameof(MediatorUpdated)]     = typeof(MediatorUpdated),
        [nameof(MediatorActivated)]   = typeof(MediatorActivated),
        [nameof(MediatorDeactivated)] = typeof(MediatorDeactivated),
        [nameof(MediatorLimitSet)]    = typeof(MediatorLimitSet),

        // Warna (dealer-scoped) events
        [nameof(WarnaCreated)]     = typeof(WarnaCreated),
        [nameof(WarnaUpdated)]     = typeof(WarnaUpdated),
        [nameof(WarnaActivated)]   = typeof(WarnaActivated),
        [nameof(WarnaDeactivated)] = typeof(WarnaDeactivated),

        // GrupTipeMotor (dealer-scoped) events
        [nameof(GrupTipeMotorCreated)]     = typeof(GrupTipeMotorCreated),
        [nameof(GrupTipeMotorActivated)]   = typeof(GrupTipeMotorActivated),
        [nameof(GrupTipeMotorDeactivated)] = typeof(GrupTipeMotorDeactivated),

        // TipeMotor (dealer-scoped) events
        [nameof(TipeMotorCreated)]     = typeof(TipeMotorCreated),
        [nameof(TipeMotorUpdated)]     = typeof(TipeMotorUpdated),
        [nameof(TipeMotorActivated)]   = typeof(TipeMotorActivated),
        [nameof(TipeMotorDeactivated)] = typeof(TipeMotorDeactivated),
        [nameof(TipeMotorColorsSet)]   = typeof(TipeMotorColorsSet),

        // Kelengkapan (dealer-scoped) events
        [nameof(KelengkapanCreated)]     = typeof(KelengkapanCreated),
        [nameof(KelengkapanUpdated)]     = typeof(KelengkapanUpdated),
        [nameof(KelengkapanActivated)]   = typeof(KelengkapanActivated),
        [nameof(KelengkapanDeactivated)] = typeof(KelengkapanDeactivated),
    };

    public static Type Resolve(string eventType) =>
        Map.TryGetValue(eventType, out var type)
            ? type
            : throw new InvalidOperationException($"Unknown event type: '{eventType}'.");

    public static string GetName(IDomainEvent evt) => evt.GetType().Name;
}

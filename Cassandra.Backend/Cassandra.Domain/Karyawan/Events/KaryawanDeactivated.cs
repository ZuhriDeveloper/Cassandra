using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanDeactivated(
    KaryawanId KaryawanId,
    string     DeactivatedBy,
    DateTime   OccurredAt) : IDomainEvent;

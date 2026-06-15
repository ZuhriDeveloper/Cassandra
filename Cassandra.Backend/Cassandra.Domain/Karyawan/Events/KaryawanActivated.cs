using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanActivated(
    KaryawanId KaryawanId,
    string     ActivatedBy,
    DateTime   OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanLimitSet(
    KaryawanId KaryawanId,
    decimal    SalesLimit,
    string     SetBy,
    DateTime   OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanResigned(
    KaryawanId KaryawanId,
    DateOnly   ResignDate,
    string     RecordedBy,
    DateTime   OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanUpdated(
    KaryawanId KaryawanId,
    string     Name,
    string     Email,
    string     Phone,
    string?    PhoneAlt,
    string     Address,
    Guid       JabatanId,
    string     UpdatedBy,
    DateTime   OccurredAt) : IDomainEvent;

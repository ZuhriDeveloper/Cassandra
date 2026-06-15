using Cassandra.Domain.Common;

namespace Cassandra.Domain.Karyawan.Events;

public record KaryawanCreated(
    KaryawanId KaryawanId,
    Guid       DealerId,
    string     Name,
    string     Email,
    string     KtpNumber,
    Gender     Gender,
    DateOnly   HireDate,
    string     Phone,
    string?    PhoneAlt,
    string     Address,
    decimal    SalesLimit,
    Guid       JabatanId,
    string     CreatedBy,
    DateTime   OccurredAt) : IDomainEvent;

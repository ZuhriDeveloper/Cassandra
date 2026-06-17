using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stnk.Events;

public record StnkCreated(
    StnkId     StnkId,
    Guid       DealerId,
    Guid       RegistrasiPenjualanId,
    DateOnly   FakturDate,
    string     FakturName,
    string     FakturAddress,
    string     CreatedBy,
    DateTime   OccurredAt) : IDomainEvent;

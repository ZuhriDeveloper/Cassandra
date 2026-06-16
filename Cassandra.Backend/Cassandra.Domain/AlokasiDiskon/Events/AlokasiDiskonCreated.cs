using Cassandra.Domain.Common;

namespace Cassandra.Domain.AlokasiDiskon.Events;

public record AlokasiDiskonCreated(
    AlokasiDiskonId AlokasiDiskonId,
    Guid DealerId,
    Guid KaryawanId,
    string DiscountLevel,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.AlokasiDiskon.Events;

public record AlokasiDiskonUpdated(
    AlokasiDiskonId AlokasiDiskonId,
    string DiscountLevel,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

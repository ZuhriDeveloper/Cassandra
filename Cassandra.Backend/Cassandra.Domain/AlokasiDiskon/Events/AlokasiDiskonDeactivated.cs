using Cassandra.Domain.Common;

namespace Cassandra.Domain.AlokasiDiskon.Events;

public record AlokasiDiskonDeactivated(
    AlokasiDiskonId AlokasiDiskonId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

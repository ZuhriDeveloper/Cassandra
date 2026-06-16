using Cassandra.Domain.Common;

namespace Cassandra.Domain.AlokasiDiskon.Events;

public record AlokasiDiskonActivated(
    AlokasiDiskonId AlokasiDiskonId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

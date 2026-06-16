using Cassandra.Domain.Common;

namespace Cassandra.Domain.BiayaBiroJasa.Events;

public record BiayaBiroJasaActivated(
    BiayaBiroJasaId BiayaBiroJasaId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

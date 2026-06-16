using Cassandra.Domain.Common;

namespace Cassandra.Domain.BiayaBiroJasa.Events;

public record BiayaBiroJasaDeactivated(
    BiayaBiroJasaId BiayaBiroJasaId,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

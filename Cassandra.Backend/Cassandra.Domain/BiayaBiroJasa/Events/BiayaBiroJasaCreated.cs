using Cassandra.Domain.Common;

namespace Cassandra.Domain.BiayaBiroJasa.Events;

public record BiayaBiroJasaCreated(
    BiayaBiroJasaId BiayaBiroJasaId,
    Guid DealerId,
    Guid SamsatId,
    Guid BiroId,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;

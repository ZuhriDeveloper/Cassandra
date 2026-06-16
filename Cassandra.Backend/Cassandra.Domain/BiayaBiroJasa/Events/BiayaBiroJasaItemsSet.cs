using Cassandra.Domain.Common;

namespace Cassandra.Domain.BiayaBiroJasa.Events;

public record BiayaBiroJasaItemsSet(
    BiayaBiroJasaId BiayaBiroJasaId,
    IReadOnlyList<BiayaBiroJasaItemValue> Items,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

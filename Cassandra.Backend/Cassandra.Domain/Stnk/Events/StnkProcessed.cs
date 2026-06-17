using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stnk.Events;

public record StnkProcessed(
    StnkId   StnkId,
    DateOnly ProcessDate,
    Guid     BiroId,
    string   InvoiceNumber,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

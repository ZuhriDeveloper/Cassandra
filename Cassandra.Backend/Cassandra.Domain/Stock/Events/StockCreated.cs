using Cassandra.Domain.Common;

namespace Cassandra.Domain.Stock.Events;

public record StockCreated(
    StockId Id,
    Guid DealerId,
    string NoMesin,
    string NoRangka,
    Guid TipeMotorId,
    Guid WarnaId,
    Guid KiosId,
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    string AssemblyYear,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;

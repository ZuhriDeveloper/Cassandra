using Cassandra.Domain.Common;

namespace Cassandra.Domain.SoPenerimaan.Events;

public record SoPenerimaanCreated(
    SoPenerimaanId Id,
    Guid DealerId,
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    IReadOnlyList<SoPenerimaanItemMotorValue> MotorItems,
    IReadOnlyList<SoPenerimaanItemKelengkapanValue> KelengkapanItems,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;

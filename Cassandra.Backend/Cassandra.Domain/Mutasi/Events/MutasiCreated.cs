using Cassandra.Domain.Common;

namespace Cassandra.Domain.Mutasi.Events;

public record MutasiCreated(
    MutasiId Id,
    Guid DealerId,
    string MutasiNumber,
    DateOnly MutasiDate,
    Guid SourceKiosId,
    Guid DestinationKiosId,
    IReadOnlyList<string> EngineNumbers,
    IReadOnlyList<MutasiKelengkapanValue> KelengkapanItems,
    string CreatedBy,
    DateTime OccurredAt) : IDomainEvent;

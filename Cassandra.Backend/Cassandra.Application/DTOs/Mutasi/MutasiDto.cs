namespace Cassandra.Application.DTOs.Mutasi;

public record MutasiDto(
    Guid Id,
    string MutasiNumber,
    DateOnly MutasiDate,
    Guid SourceKiosId,
    Guid DestinationKiosId,
    bool IsActive,
    List<string>? EngineNumbers,
    List<MutasiKelengkapanItemDto>? KelengkapanItems);

public record MutasiKelengkapanItemDto(string KelengkapanName, int Qty);

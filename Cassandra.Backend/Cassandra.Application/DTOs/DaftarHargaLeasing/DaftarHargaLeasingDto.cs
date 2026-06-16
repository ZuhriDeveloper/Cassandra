namespace Cassandra.Application.DTOs.DaftarHargaLeasing;

public record DaftarHargaLeasingItemDto(Guid GrupTipeMotorId, decimal Subsidi, decimal Incentive, decimal LainLain, decimal Total);

public record DaftarHargaLeasingDto(
    Guid Id,
    string Name,
    Guid GlobalLeasingId,
    Guid GrupTenorId,
    bool IsActive,
    IReadOnlyList<DaftarHargaLeasingItemDto> Items);

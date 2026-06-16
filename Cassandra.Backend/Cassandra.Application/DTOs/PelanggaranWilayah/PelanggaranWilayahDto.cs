namespace Cassandra.Application.DTOs.PelanggaranWilayah;

public record PelanggaranWilayahDto(
    Guid Id,
    string AreaCode,
    decimal ExtraFee,
    bool IsActive);

namespace Cassandra.Application.DTOs.SoPenerimaan;

public record SoPenerimaanDto(
    Guid Id,
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    List<SoPenerimaanItemMotorDto>? MotorItems,
    List<SoPenerimaanItemKelengkapanDto>? KelengkapanItems);

public record SoPenerimaanItemMotorDto(
    Guid TipeMotorId,
    Guid WarnaId,
    string NoMesin,
    string NoRangka,
    Guid KiosId,
    string AssemblyYear);

public record SoPenerimaanItemKelengkapanDto(Guid KelengkapanId, int Qty, string? Notes);

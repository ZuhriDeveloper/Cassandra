namespace Cassandra.Application.DTOs.Stock;

public record StockDto(
    Guid Id,
    string NoMesin,
    string NoRangka,
    Guid TipeMotorId,
    Guid WarnaId,
    Guid KiosId,
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    string AssemblyYear,
    string Status,
    bool IsActive);

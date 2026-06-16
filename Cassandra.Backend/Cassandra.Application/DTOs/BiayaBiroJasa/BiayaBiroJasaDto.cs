namespace Cassandra.Application.DTOs.BiayaBiroJasa;

public record BiayaBiroJasaDto(
    Guid Id,
    Guid SamsatId,
    Guid BiroId,
    bool IsActive,
    IReadOnlyList<BiayaBiroJasaItemDto> Items);

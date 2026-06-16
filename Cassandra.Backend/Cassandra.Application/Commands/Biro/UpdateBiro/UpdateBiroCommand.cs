namespace Cassandra.Application.Commands.Biro.UpdateBiro;

public record UpdateBiroCommand(
    Guid Id,
    string Name,
    string? Phone,
    string? Fax,
    string? Pic,
    string? Address,
    decimal PphRate,
    string UpdatedBy);

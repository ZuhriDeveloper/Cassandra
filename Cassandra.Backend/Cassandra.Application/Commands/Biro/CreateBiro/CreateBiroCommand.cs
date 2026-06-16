namespace Cassandra.Application.Commands.Biro.CreateBiro;

public record CreateBiroCommand(
    string Code,
    string Name,
    string? Phone,
    string? Fax,
    string? Pic,
    string? Address,
    decimal PphRate,
    string CreatedBy);

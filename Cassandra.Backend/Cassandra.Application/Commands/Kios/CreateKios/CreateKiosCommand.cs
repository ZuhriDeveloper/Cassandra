namespace Cassandra.Application.Commands.Kios.CreateKios;

public record CreateKiosCommand(
    string  Code,
    string  Name,
    string  Phone,
    string? Fax,
    string  Address,
    Guid    PicKaryawanId,
    decimal Limit,
    string  CreatedBy);

namespace Cassandra.Application.Commands.Kios.UpdateKios;

public record UpdateKiosCommand(
    Guid    Id,
    string  Name,
    string  Phone,
    string? Fax,
    string  Address,
    Guid    PicKaryawanId,
    string  UpdatedBy);

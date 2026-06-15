namespace Cassandra.Application.Commands.Karyawan.UpdateKaryawan;

public record UpdateKaryawanCommand(
    Guid    Id,
    string  Name,
    string  Email,
    string  Phone,
    string? PhoneAlt,
    string  Address,
    Guid    JabatanId,
    string  UpdatedBy);

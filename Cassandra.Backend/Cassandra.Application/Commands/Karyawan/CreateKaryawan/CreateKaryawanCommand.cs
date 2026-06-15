using Cassandra.Domain.Karyawan;

namespace Cassandra.Application.Commands.Karyawan.CreateKaryawan;

public record CreateKaryawanCommand(
    string   Name,
    string   Email,
    string   KtpNumber,
    Gender   Gender,
    DateOnly HireDate,
    string   Phone,
    string?  PhoneAlt,
    string   Address,
    decimal  SalesLimit,
    Guid     JabatanId,
    string   CreatedBy);

namespace Cassandra.Application.DTOs.Karyawan;

public record KaryawanDto(
    Guid      Id,
    string    Name,
    string    Email,
    string    KtpNumber,
    string    Gender,
    DateOnly  HireDate,
    DateOnly? ResignDate,
    string    Phone,
    string?   PhoneAlt,
    string    Address,
    decimal   SalesLimit,
    Guid      JabatanId,
    bool      IsActive);

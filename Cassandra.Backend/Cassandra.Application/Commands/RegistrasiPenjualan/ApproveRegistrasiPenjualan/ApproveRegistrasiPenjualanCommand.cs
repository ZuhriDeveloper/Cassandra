namespace Cassandra.Application.Commands.RegistrasiPenjualan.ApproveRegistrasiPenjualan;

public record ApproveRegistrasiPenjualanCommand(
    Guid    Id,
    decimal ApprovedDiscount,
    string  ApprovedBy);

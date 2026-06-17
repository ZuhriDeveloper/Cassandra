namespace Cassandra.Application.Commands.RegistrasiPenjualan.VoidRegistrasiPenjualan;

public record VoidRegistrasiPenjualanCommand(
    Guid   Id,
    string VoidedBy);

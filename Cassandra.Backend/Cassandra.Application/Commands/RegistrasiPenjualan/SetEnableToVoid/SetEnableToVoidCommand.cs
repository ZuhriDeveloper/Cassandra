namespace Cassandra.Application.Commands.RegistrasiPenjualan.SetEnableToVoid;

public record SetEnableToVoidCommand(
    Guid   Id,
    bool   EnableToVoid,
    string UpdatedBy);

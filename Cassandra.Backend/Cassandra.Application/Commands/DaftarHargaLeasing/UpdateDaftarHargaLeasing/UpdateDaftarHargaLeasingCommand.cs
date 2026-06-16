namespace Cassandra.Application.Commands.DaftarHargaLeasing.UpdateDaftarHargaLeasing;

public record UpdateDaftarHargaLeasingCommand(Guid Id, string Name, Guid GlobalLeasingId, Guid GrupTenorId, string UpdatedBy);

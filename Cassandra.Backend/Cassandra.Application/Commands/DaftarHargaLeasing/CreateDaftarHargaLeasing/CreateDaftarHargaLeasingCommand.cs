namespace Cassandra.Application.Commands.DaftarHargaLeasing.CreateDaftarHargaLeasing;

public record CreateDaftarHargaLeasingCommand(string Name, Guid GlobalLeasingId, Guid GrupTenorId, string CreatedBy);

namespace Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingStatus;

public record SetDaftarHargaLeasingStatusCommand(Guid Id, bool IsActive, string ActionBy);

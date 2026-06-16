namespace Cassandra.Application.Commands.PelanggaranWilayah.SetPelanggaranWilayahStatus;

public record SetPelanggaranWilayahStatusCommand(Guid Id, bool IsActive, string UpdatedBy);

namespace Cassandra.Application.Commands.Karyawan.SetKaryawanStatus;

public record SetKaryawanStatusCommand(Guid Id, bool IsActive, string ActionBy);

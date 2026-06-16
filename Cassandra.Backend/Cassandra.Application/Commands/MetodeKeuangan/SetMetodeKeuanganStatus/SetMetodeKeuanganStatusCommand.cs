namespace Cassandra.Application.Commands.MetodeKeuangan.SetMetodeKeuanganStatus;

public record SetMetodeKeuanganStatusCommand(Guid Id, bool IsActive, string ActionBy);

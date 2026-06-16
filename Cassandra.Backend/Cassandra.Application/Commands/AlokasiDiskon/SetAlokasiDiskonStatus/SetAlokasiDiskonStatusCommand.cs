namespace Cassandra.Application.Commands.AlokasiDiskon.SetAlokasiDiskonStatus;

public record SetAlokasiDiskonStatusCommand(Guid Id, bool IsActive, string ActionBy);

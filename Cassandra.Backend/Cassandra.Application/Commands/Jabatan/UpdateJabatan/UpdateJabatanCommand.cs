namespace Cassandra.Application.Commands.Jabatan.UpdateJabatan;

public record UpdateJabatanCommand(Guid Id, string Name, string Description, string UpdatedBy);

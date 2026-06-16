namespace Cassandra.Application.Commands.BiayaBiroJasa.CreateBiayaBiroJasa;

public record CreateBiayaBiroJasaCommand(Guid SamsatId, Guid BiroId, string CreatedBy);

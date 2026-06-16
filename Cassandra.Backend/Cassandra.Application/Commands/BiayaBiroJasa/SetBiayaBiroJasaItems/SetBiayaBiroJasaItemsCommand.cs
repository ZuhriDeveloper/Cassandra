using Cassandra.Domain.BiayaBiroJasa;

namespace Cassandra.Application.Commands.BiayaBiroJasa.SetBiayaBiroJasaItems;

public record SetBiayaBiroJasaItemsCommand(Guid Id, IReadOnlyList<BiayaBiroJasaItemValue> Items, string UpdatedBy);

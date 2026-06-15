namespace Cassandra.Application.Commands.Dealers.RenameDealer;

public record RenameDealerCommand(Guid DealerId, string Name, string RenamedBy);

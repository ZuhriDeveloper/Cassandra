namespace Cassandra.Application.Commands.Dealers.RegisterDealer;

public record RegisterDealerCommand(string Name, string Code, string RegisteredBy);

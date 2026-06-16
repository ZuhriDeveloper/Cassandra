namespace Cassandra.Application.Commands.Samsat.SetSamsatCities;

public record SetSamsatCitiesCommand(Guid Id, IReadOnlyList<string> Cities, string UpdatedBy);

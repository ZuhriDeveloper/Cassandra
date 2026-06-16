namespace Cassandra.Application.Commands.Df.SetDf;

public record SetDfCommand(decimal Discount, decimal Interest, DateOnly StartDate, string UpdatedBy);

namespace Cassandra.Application.Commands.Stnk.HandoverStnk;

public record HandoverStnkCommand(
    Guid     StnkId,
    DateOnly HandoverDate,
    string   StnkReceiver,
    string   UpdatedBy);

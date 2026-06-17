namespace Cassandra.Application.Commands.Bpkb.HandoverBpkb;

public record HandoverBpkbCommand(
    Guid     BpkbId,
    DateOnly HandoverDate,
    string   Receiver,
    string   UpdatedBy);

namespace Cassandra.Application.Commands.Bpkb.ReceiveBpkb;

public record ReceiveBpkbCommand(
    Guid     BpkbId,
    DateOnly ReceiveDate,
    string   BpkbNumber,
    string   BookNumber,
    string   UpdatedBy);

namespace Cassandra.Application.DTOs.Bpkb;

public record BpkbDto(
    Guid     Id,
    Guid     RegistrasiPenjualanId,
    Guid     StnkId,
    string   Status,
    DateOnly RequestDate,
    string?  BpkbNumber,
    string?  BookNumber,
    DateOnly? ReceiveDate,
    DateOnly? HandoverDate,
    string?  Receiver);

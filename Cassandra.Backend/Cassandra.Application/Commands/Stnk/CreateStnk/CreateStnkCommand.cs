namespace Cassandra.Application.Commands.Stnk.CreateStnk;

public record CreateStnkCommand(
    Guid     RegistrasiPenjualanId,
    DateOnly FakturDate,
    string   FakturName,
    string   FakturAddress,
    string   CreatedBy);

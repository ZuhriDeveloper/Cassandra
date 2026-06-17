namespace Cassandra.Domain.SoPenerimaan;

public record SoPenerimaanItemMotorValue(
    Guid TipeMotorId,
    Guid WarnaId,
    string NoMesin,
    string NoRangka,
    Guid KiosId,
    string AssemblyYear);

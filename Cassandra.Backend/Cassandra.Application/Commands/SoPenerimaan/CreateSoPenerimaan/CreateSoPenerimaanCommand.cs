namespace Cassandra.Application.Commands.SoPenerimaan.CreateSoPenerimaan;

public record CreateSoPenerimaanItemMotorRequest(
    Guid TipeMotorId,
    Guid WarnaId,
    string NoMesin,
    string NoRangka,
    Guid KiosId,
    string AssemblyYear);

public record CreateSoPenerimaanItemKelengkapanRequest(
    Guid KelengkapanId,
    int Qty,
    string? Notes);

public record CreateSoPenerimaanCommand(
    string SuratJalanId,
    DateOnly SuratJalanDate,
    Guid SoId,
    List<CreateSoPenerimaanItemMotorRequest> MotorItems,
    List<CreateSoPenerimaanItemKelengkapanRequest> KelengkapanItems,
    string CreatedBy);

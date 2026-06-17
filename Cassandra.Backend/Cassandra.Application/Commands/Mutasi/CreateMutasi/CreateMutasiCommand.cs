namespace Cassandra.Application.Commands.Mutasi.CreateMutasi;

public record CreateMutasiKelengkapanItemRequest(string KelengkapanName, int Qty);

public record CreateMutasiCommand(
    string MutasiNumber,
    DateOnly MutasiDate,
    Guid SourceKiosId,
    Guid DestinationKiosId,
    List<string> EngineNumbers,
    List<CreateMutasiKelengkapanItemRequest> KelengkapanItems,
    string CreatedBy);

namespace Cassandra.Application.Commands.DaftarHargaLeasing.SetDaftarHargaLeasingItems;

public record SetDaftarHargaLeasingItem(Guid GrupTipeMotorId, decimal Subsidi, decimal Incentive, decimal LainLain);

public record SetDaftarHargaLeasingItemsCommand(Guid Id, List<SetDaftarHargaLeasingItem> Items, string UpdatedBy);

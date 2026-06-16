namespace Cassandra.Application.Commands.PelanggaranWilayah.UpdatePelanggaranWilayah;

public record UpdatePelanggaranWilayahCommand(Guid Id, decimal ExtraFee, string UpdatedBy);

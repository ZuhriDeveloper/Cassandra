namespace Cassandra.Application.Commands.PelanggaranWilayah.CreatePelanggaranWilayah;

public record CreatePelanggaranWilayahCommand(string AreaCode, decimal ExtraFee, string CreatedBy);

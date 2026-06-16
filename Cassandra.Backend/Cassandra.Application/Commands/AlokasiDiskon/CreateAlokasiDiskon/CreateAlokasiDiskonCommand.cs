namespace Cassandra.Application.Commands.AlokasiDiskon.CreateAlokasiDiskon;

public record CreateAlokasiDiskonCommand(Guid KaryawanId, string DiscountLevel, string CreatedBy);

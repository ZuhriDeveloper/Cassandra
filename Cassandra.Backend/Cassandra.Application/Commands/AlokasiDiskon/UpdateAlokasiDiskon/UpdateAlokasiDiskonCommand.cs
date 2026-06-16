namespace Cassandra.Application.Commands.AlokasiDiskon.UpdateAlokasiDiskon;

public record UpdateAlokasiDiskonCommand(Guid Id, string DiscountLevel, string UpdatedBy);

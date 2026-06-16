namespace Cassandra.Application.DTOs.AlokasiDiskon;

public record AlokasiDiskonDto(Guid Id, Guid KaryawanId, string DiscountLevel, bool IsActive);

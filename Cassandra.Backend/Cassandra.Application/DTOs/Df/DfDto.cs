namespace Cassandra.Application.DTOs.Df;

public record DfDto(Guid Id, decimal Discount, decimal Interest, DateOnly StartDate);

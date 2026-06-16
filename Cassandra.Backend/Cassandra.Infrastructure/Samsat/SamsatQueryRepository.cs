using Cassandra.Application.Contracts.Samsat;
using Cassandra.Application.DTOs.Samsat;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Samsat;

public class SamsatQueryRepository(AppDbContext context) : ISamsatQueryRepository
{
    public async Task<SamsatDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var header = await context.SamsatReadModels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (header is null) return null;

        var cities = await context.SamsatCityReadModels
            .AsNoTracking()
            .Where(x => x.SamsatId == id)
            .Select(x => x.City)
            .ToListAsync(ct);

        return new SamsatDto(header.Id, header.Name, header.IsActive, cities);
    }

    public async Task<IReadOnlyList<SamsatDto>> GetAllAsync(CancellationToken ct = default)
    {
        var headers = await context.SamsatReadModels
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        if (headers.Count == 0) return [];

        var ids = headers.Select(h => h.Id).ToList();
        var allCities = await context.SamsatCityReadModels
            .AsNoTracking()
            .Where(x => ids.Contains(x.SamsatId))
            .ToListAsync(ct);

        return headers.Select(h =>
        {
            var cities = allCities
                .Where(x => x.SamsatId == h.Id)
                .Select(x => x.City)
                .ToList();
            return new SamsatDto(h.Id, h.Name, h.IsActive, cities);
        }).ToList();
    }

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
        => context.SamsatReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower(), ct);

    public Task<bool> NameExistsExcludingAsync(string name, Guid excludeId, CancellationToken ct = default)
        => context.SamsatReadModels.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != excludeId, ct);
}

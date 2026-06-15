using Cassandra.Application.Contracts.Karyawan;
using Cassandra.Application.DTOs.Karyawan;
using Cassandra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.Karyawan;

public class KaryawanQueryRepository(AppDbContext context) : IKaryawanQueryRepository
{
    public async Task<KaryawanDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.KaryawanReadModels
            .AsNoTracking()
            .Where(k => k.Id == id)
            .Select(k => new KaryawanDto(
                k.Id, k.Name, k.Email, k.KtpNumber, k.Gender,
                k.HireDate, k.ResignDate, k.Phone, k.PhoneAlt,
                k.Address, k.SalesLimit, k.JabatanId, k.IsActive))
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<KaryawanDto>> GetAllAsync(CancellationToken ct = default)
        => await context.KaryawanReadModels
            .AsNoTracking()
            .OrderBy(k => k.Name)
            .Select(k => new KaryawanDto(
                k.Id, k.Name, k.Email, k.KtpNumber, k.Gender,
                k.HireDate, k.ResignDate, k.Phone, k.PhoneAlt,
                k.Address, k.SalesLimit, k.JabatanId, k.IsActive))
            .ToListAsync(ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => context.KaryawanReadModels
            .AnyAsync(k => k.Email.ToLower() == email.ToLower(), ct);

    public Task<bool> EmailExistsExcludingAsync(string email, Guid excludeId, CancellationToken ct = default)
        => context.KaryawanReadModels
            .AnyAsync(k => k.Email.ToLower() == email.ToLower() && k.Id != excludeId, ct);
}

using Cassandra.Application.DTOs.SoPenerimaan;

namespace Cassandra.Application.Contracts.SoPenerimaan;

public interface ISoPenerimaanQueryRepository
{
    Task<IReadOnlyList<SoPenerimaanDto>> GetAllAsync(CancellationToken ct = default);
    Task<SoPenerimaanDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SoPenerimaanDto?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<bool> SuratJalanIdExistsAsync(string suratJalanId, CancellationToken ct = default);
    Task<bool> HasPenerimaanForSoAsync(Guid soId, CancellationToken ct = default);
}

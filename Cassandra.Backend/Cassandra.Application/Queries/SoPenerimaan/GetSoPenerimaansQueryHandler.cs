using Cassandra.Application.Contracts.SoPenerimaan;
using Cassandra.Application.DTOs.SoPenerimaan;

namespace Cassandra.Application.Queries.SoPenerimaan;

public record GetSoPenerimaansQuery;
public record GetSoPenerimaanByIdQuery(Guid Id);

public class GetSoPenerimaansQueryHandler(ISoPenerimaanQueryRepository queryRepository)
{
    public Task<IReadOnlyList<SoPenerimaanDto>> HandleAsync(GetSoPenerimaansQuery query, CancellationToken ct = default)
        => queryRepository.GetAllAsync(ct);

    public Task<SoPenerimaanDto?> HandleByIdAsync(GetSoPenerimaanByIdQuery query, CancellationToken ct = default)
        => queryRepository.GetWithItemsAsync(query.Id, ct);
}

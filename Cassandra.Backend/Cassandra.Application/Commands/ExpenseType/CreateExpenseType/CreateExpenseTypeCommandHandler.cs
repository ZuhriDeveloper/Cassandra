using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.ExpenseType.CreateExpenseType;

public class CreateExpenseTypeCommandHandler(
    IExpenseTypeRepository repository,
    IExpenseTypeQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateExpenseTypeCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var code = command.Code.Trim().ToUpper();

        if (await queryRepository.CodeExistsAsync(code, ct))
            throw new DomainException($"Kode tipe biaya '{code}' sudah ada.");
        if (await queryRepository.NameExistsAsync(command.Name.Trim(), ct))
            throw new DomainException($"Nama tipe biaya '{command.Name.Trim()}' sudah ada.");

        var et = Domain.ExpenseType.ExpenseType.Create(command.Code, command.Name, command.CreatedBy, dealerId);
        await repository.SaveAsync(et, ct);
        return et.Id.Value;
    }
}

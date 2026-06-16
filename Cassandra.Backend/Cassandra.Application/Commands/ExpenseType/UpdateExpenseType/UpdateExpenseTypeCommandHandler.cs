using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType;

namespace Cassandra.Application.Commands.ExpenseType.UpdateExpenseType;

public class UpdateExpenseTypeCommandHandler(
    IExpenseTypeRepository repository,
    IExpenseTypeQueryRepository queryRepository)
{
    public async Task HandleAsync(UpdateExpenseTypeCommand command, CancellationToken ct = default)
    {
        var et = await repository.GetByIdAsync(ExpenseTypeId.From(command.Id), ct)
            ?? throw new DomainException("Tipe biaya tidak ditemukan.");

        if (await queryRepository.NameExistsExcludingAsync(command.Name.Trim(), command.Id, ct))
            throw new DomainException($"Nama tipe biaya '{command.Name.Trim()}' sudah ada.");

        et.Update(command.Name, command.UpdatedBy);
        await repository.SaveAsync(et, ct);
    }
}

using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType;

namespace Cassandra.Application.Commands.ExpenseType.SetExpenseTypeStatus;

public class SetExpenseTypeStatusCommandHandler(IExpenseTypeRepository repository)
{
    public async Task HandleAsync(SetExpenseTypeStatusCommand command, CancellationToken ct = default)
    {
        var et = await repository.GetByIdAsync(ExpenseTypeId.From(command.Id), ct)
            ?? throw new DomainException("Tipe biaya tidak ditemukan.");

        if (command.IsActive)
            et.Activate(command.UpdatedBy);
        else
            et.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(et, ct);
    }
}

using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;

namespace Cassandra.Application.Commands.TipeMotor.SetTipeMotorStatus;

public class SetTipeMotorStatusCommandHandler(ITipeMotorRepository repository)
{
    public async Task HandleAsync(SetTipeMotorStatusCommand command, CancellationToken ct = default)
    {
        var tipe = await repository.GetByIdAsync(TipeMotorId.From(command.Id), ct)
            ?? throw new DomainException($"Tipe motor dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            tipe.Activate(command.UpdatedBy);
        else
            tipe.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(tipe, ct);
    }
}

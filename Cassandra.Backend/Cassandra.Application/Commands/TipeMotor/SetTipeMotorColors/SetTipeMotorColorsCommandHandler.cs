using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;

namespace Cassandra.Application.Commands.TipeMotor.SetTipeMotorColors;

public class SetTipeMotorColorsCommandHandler(ITipeMotorRepository repository)
{
    public async Task HandleAsync(SetTipeMotorColorsCommand command, CancellationToken ct = default)
    {
        var tipe = await repository.GetByIdAsync(TipeMotorId.From(command.Id), ct)
            ?? throw new DomainException($"Tipe motor dengan id '{command.Id}' tidak ditemukan.");

        tipe.SetColors(command.WarnaIds, command.UpdatedBy);
        await repository.SaveAsync(tipe, ct);
    }
}

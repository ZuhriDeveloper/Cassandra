using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.GrupTipeMotor;

namespace Cassandra.Application.Commands.GrupTipeMotor.SetGrupTipeMotorStatus;

public class SetGrupTipeMotorStatusCommandHandler(IGrupTipeMotorRepository repository)
{
    public async Task HandleAsync(SetGrupTipeMotorStatusCommand command, CancellationToken ct = default)
    {
        var grup = await repository.GetByIdAsync(GrupTipeMotorId.From(command.Id), ct)
            ?? throw new DomainException($"Grup tipe motor dengan id '{command.Id}' tidak ditemukan.");

        if (command.IsActive)
            grup.Activate(command.UpdatedBy);
        else
            grup.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(grup, ct);
    }
}

using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Domain.Common;
using Cassandra.Domain.TipeMotor;

namespace Cassandra.Application.Commands.TipeMotor.UpdateTipeMotor;

public class UpdateTipeMotorCommandHandler(ITipeMotorRepository repository)
{
    public async Task HandleAsync(UpdateTipeMotorCommand command, CancellationToken ct = default)
    {
        var tipe = await repository.GetByIdAsync(TipeMotorId.From(command.Id), ct)
            ?? throw new DomainException($"Tipe motor dengan id '{command.Id}' tidak ditemukan.");

        tipe.Update(
            command.GrupTipeMotorId,
            command.ShortName,
            command.ProductCode,
            command.WmsCode,
            command.AhmCode,
            command.EngineNumberFormat,
            command.ChassisNumberFormat,
            command.NettPrice,
            command.OrJakarta,
            command.OrTangerang,
            command.BbnJakarta,
            command.BbnTangerang,
            command.UpdatedBy);

        await repository.SaveAsync(tipe, ct);
    }
}

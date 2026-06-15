using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.TipeMotor;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.TipeMotor.CreateTipeMotor;

public class CreateTipeMotorCommandHandler(
    ITipeMotorRepository repository,
    ITipeMotorQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateTipeMotorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.CodeExistsAsync(command.Code.Trim().ToUpper(), ct))
            throw new DomainException($"Kode tipe motor '{command.Code}' sudah digunakan.");

        var tipe = Domain.TipeMotor.TipeMotor.Create(
            command.Code,
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
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(tipe, ct);
        return tipe.Id.Value;
    }
}

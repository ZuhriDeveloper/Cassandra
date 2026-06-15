using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.GrupTipeMotor;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.GrupTipeMotor.CreateGrupTipeMotor;

public class CreateGrupTipeMotorCommandHandler(
    IGrupTipeMotorRepository repository,
    IGrupTipeMotorQueryRepository queryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateGrupTipeMotorCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;

        if (await queryRepository.CodeExistsAsync(command.Code.Trim().ToUpper(), ct))
            throw new DomainException($"Kode grup tipe motor '{command.Code}' sudah digunakan.");

        var grup = Domain.GrupTipeMotor.GrupTipeMotor.Create(command.Code, command.CreatedBy, dealerId);
        await repository.SaveAsync(grup, ct);
        return grup.Id.Value;
    }
}

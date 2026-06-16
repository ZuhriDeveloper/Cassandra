using Cassandra.Application.Contracts.Biro;
using Cassandra.Domain.Biro;
using Cassandra.Domain.Common;

namespace Cassandra.Application.Commands.Biro.SetBiroStatus;

public class SetBiroStatusCommandHandler(IBiroRepository repository)
{
    public async Task HandleAsync(SetBiroStatusCommand command, CancellationToken ct = default)
    {
        var biro = await repository.GetByIdAsync(BiroId.From(command.Id), ct)
            ?? throw new DomainException("Biro tidak ditemukan.");

        if (command.IsActive)
            biro.Activate(command.UpdatedBy);
        else
            biro.Deactivate(command.UpdatedBy);

        await repository.SaveAsync(biro, ct);
    }
}

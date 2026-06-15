using Cassandra.Application.Contracts.Dealers;
using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers;

namespace Cassandra.Application.Commands.Dealers.RegisterDealer;

public class RegisterDealerCommandHandler(
    IDealerRepository repository,
    IDealerQueryRepository queryRepository)
{
    public async Task<Guid> HandleAsync(RegisterDealerCommand command, CancellationToken ct = default)
    {
        if (await queryRepository.CodeExistsAsync(command.Code.Trim(), ct))
            throw new DomainException($"A dealer with code '{command.Code}' already exists.");

        var dealer = Dealer.Register(command.Name, command.Code, command.RegisteredBy);
        await repository.SaveAsync(dealer, ct);
        return dealer.Id.Value;
    }
}

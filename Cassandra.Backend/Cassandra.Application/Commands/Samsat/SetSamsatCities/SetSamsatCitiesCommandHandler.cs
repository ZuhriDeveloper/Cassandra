using Cassandra.Application.Contracts.Samsat;
using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat;

namespace Cassandra.Application.Commands.Samsat.SetSamsatCities;

public class SetSamsatCitiesCommandHandler(ISamsatRepository repository)
{
    public async Task HandleAsync(SetSamsatCitiesCommand command, CancellationToken ct = default)
    {
        var samsat = await repository.GetByIdAsync(SamsatId.From(command.Id), ct)
            ?? throw new DomainException("Samsat tidak ditemukan.");

        samsat.SetCities(command.Cities, command.UpdatedBy);
        await repository.SaveAsync(samsat, ct);
    }
}

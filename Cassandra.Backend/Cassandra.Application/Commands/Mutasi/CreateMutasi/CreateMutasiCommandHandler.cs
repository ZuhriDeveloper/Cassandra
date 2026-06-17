using Cassandra.Application.Contracts.Dealers;
using Cassandra.Application.Contracts.Mutasi;
using Cassandra.Application.Contracts.Stock;
using Cassandra.Domain.Common;
using Cassandra.Domain.Mutasi;
using Cassandra.Domain.Stock;

namespace Cassandra.Application.Commands.Mutasi.CreateMutasi;

public class CreateMutasiCommandHandler(
    IMutasiRepository repository,
    IMutasiQueryRepository queryRepository,
    IStockRepository stockRepository,
    IStockQueryRepository stockQueryRepository,
    ICurrentDealer currentDealer)
{
    public async Task<Guid> HandleAsync(CreateMutasiCommand command, CancellationToken ct = default)
    {
        var dealerId = currentDealer.DealerId;
        var mutasiNumber = command.MutasiNumber.Trim().ToUpper();

        // 1. Validate MutasiNumber unique
        if (await queryRepository.MutasiNumberExistsAsync(mutasiNumber, ct))
            throw new DomainException($"Nomor mutasi '{mutasiNumber}' sudah ada.");

        // 2. Validate SourceKiosId != DestinationKiosId
        if (command.SourceKiosId == command.DestinationKiosId)
            throw new DomainException("Kios asal dan kios tujuan tidak boleh sama.");

        // 3. For each engine number, load stock, validate and move
        foreach (var noMesin in command.EngineNumbers)
        {
            var stockDto = await stockQueryRepository.GetByNoMesinAsync(noMesin, ct)
                ?? throw new DomainException($"Stock dengan No Mesin '{noMesin}' tidak ditemukan.");

            var stock = await stockRepository.GetByIdAsync(StockId.From(stockDto.Id), ct)
                ?? throw new DomainException($"Stock dengan ID '{stockDto.Id}' tidak ditemukan.");

            if (stock.Status != StockStatus.TERSEDIA)
                throw new DomainException($"Stock '{noMesin}' tidak dalam status TERSEDIA.");

            stock.MoveToKios(command.DestinationKiosId, command.CreatedBy);
            await stockRepository.SaveAsync(stock, ct);
        }

        // 4. Create Mutasi aggregate
        var kelengkapanValues = command.KelengkapanItems
            .Select(k => new MutasiKelengkapanValue(k.KelengkapanName, k.Qty))
            .ToList();

        var mutasi = Domain.Mutasi.Mutasi.Create(
            mutasiNumber,
            command.MutasiDate,
            command.SourceKiosId,
            command.DestinationKiosId,
            command.EngineNumbers,
            kelengkapanValues,
            command.CreatedBy,
            dealerId);

        await repository.SaveAsync(mutasi, ct);
        return mutasi.Id.Value;
    }
}

using Cassandra.Domain.Common;
using Cassandra.Domain.Mutasi;
using Cassandra.Domain.Mutasi.Events;

namespace Cassandra.Tests.Mutasi;

public class MutasiTests
{
    private static readonly Guid DealerId = Guid.NewGuid();
    private static readonly Guid SourceKiosId = Guid.NewGuid();
    private static readonly Guid DestKiosId = Guid.NewGuid();

    private static Domain.Mutasi.Mutasi MakeMutasi(
        string mutasiNumber = "MUT-001",
        Guid? sourceKiosId = null,
        Guid? destKiosId = null,
        List<string>? engineNumbers = null)
    {
        return Domain.Mutasi.Mutasi.Create(
            mutasiNumber,
            DateOnly.FromDateTime(DateTime.Today),
            sourceKiosId ?? SourceKiosId,
            destKiosId ?? DestKiosId,
            engineNumbers ?? ["M001", "M002"],
            [],
            "admin", DealerId);
    }

    [Fact]
    public void Create_SetsStateCorrectly()
    {
        var mutasi = MakeMutasi();

        Assert.Single(mutasi.DomainEvents);
        var evt = Assert.IsType<MutasiCreated>(mutasi.DomainEvents[0]);

        Assert.Equal("MUT-001", evt.MutasiNumber);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(SourceKiosId, evt.SourceKiosId);
        Assert.Equal(DestKiosId, evt.DestinationKiosId);
        Assert.True(mutasi.IsActive);
        Assert.Equal(2, mutasi.EngineNumbers.Count);
    }

    [Fact]
    public void Create_NormalisesMutasiNumberToUppercase()
    {
        var mutasi = MakeMutasi(mutasiNumber: "mut-001");
        Assert.Equal("MUT-001", mutasi.MutasiNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsWhen_MutasiNumberEmpty(string mutasiNumber)
    {
        Assert.Throws<DomainException>(() => MakeMutasi(mutasiNumber: mutasiNumber));
    }

    [Fact]
    public void Create_ThrowsWhen_SameSourceAndDestKios()
    {
        var sameKiosId = Guid.NewGuid();
        Assert.Throws<DomainException>(() =>
            MakeMutasi(sourceKiosId: sameKiosId, destKiosId: sameKiosId));
    }

    [Fact]
    public void Create_ThrowsWhen_EngineNumbersEmpty()
    {
        Assert.Throws<DomainException>(() => MakeMutasi(engineNumbers: []));
    }

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var mutasi = MakeMutasi();

        var replayed = Domain.Mutasi.Mutasi.Reconstitute(mutasi.DomainEvents);

        Assert.Equal(mutasi.Id, replayed.Id);
        Assert.Equal(mutasi.MutasiNumber, replayed.MutasiNumber);
        Assert.Equal(mutasi.SourceKiosId, replayed.SourceKiosId);
        Assert.Equal(mutasi.DestinationKiosId, replayed.DestinationKiosId);
        Assert.Equal(2, replayed.EngineNumbers.Count);
        Assert.True(replayed.IsActive);
    }
}

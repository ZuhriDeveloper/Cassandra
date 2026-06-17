using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk;

namespace Cassandra.Tests.Stnk;

public class StnkAggregateTests
{
    private static readonly Guid DealerId              = Guid.NewGuid();
    private static readonly Guid RegistrasiPenjualanId = Guid.NewGuid();
    private static readonly Guid BiroId                = Guid.NewGuid();

    private static Domain.Stnk.Stnk DefaultStnk() =>
        Domain.Stnk.Stnk.Create(
            RegistrasiPenjualanId,
            DateOnly.FromDateTime(DateTime.Today),
            "Budi Santoso",
            "Jl. Merdeka No. 1, Jakarta",
            "admin",
            DealerId);

    [Fact]
    public void Create_SetsStatusReceiveFaktur()
    {
        var stnk = DefaultStnk();

        Assert.Equal(StnkStatus.RECEIVE_FAKTUR, stnk.Status);
        Assert.Equal(RegistrasiPenjualanId, stnk.RegistrasiPenjualanId);
        Assert.Equal("Budi Santoso", stnk.FakturName);
        Assert.Equal(DealerId, stnk.DealerId);
        Assert.Equal(1, stnk.Version);
    }

    [Fact]
    public void Create_Throws_WhenFakturNameEmpty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.Stnk.Stnk.Create(
                RegistrasiPenjualanId,
                DateOnly.FromDateTime(DateTime.Today),
                "",
                "Jl. Test",
                "admin",
                DealerId));
    }

    [Fact]
    public void Create_Throws_WhenFakturAddressEmpty()
    {
        Assert.Throws<DomainException>(() =>
            Domain.Stnk.Stnk.Create(
                RegistrasiPenjualanId,
                DateOnly.FromDateTime(DateTime.Today),
                "Budi",
                "",
                "admin",
                DealerId));
    }

    [Fact]
    public void Process_SetsStatusProcess()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");

        Assert.Equal(StnkStatus.PROCESS, stnk.Status);
        Assert.Equal(BiroId, stnk.BiroId);
        Assert.Equal("INV-001", stnk.InvoiceNumber);
    }

    [Fact]
    public void Process_Throws_WhenNotReceiveFaktur()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");

        // now in PROCESS — cannot process again
        Assert.Throws<DomainException>(() =>
            stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-002", "admin"));
    }

    [Fact]
    public void Receive_SetsStatusReceive()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        stnk.Receive(
            DateOnly.FromDateTime(DateTime.Today), "B1234XY", BiroId, "STNK-001",
            500_000m, 50_000m, 0m, "INV-001", "DKI", 100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
            true, "admin");

        Assert.Equal(StnkStatus.RECEIVE, stnk.Status);
        Assert.Equal("B1234XY", stnk.PlateNumber);
        Assert.Equal("STNK-001", stnk.StnkNumber);
        Assert.Equal(500_000m, stnk.StnkCost);
        Assert.Equal(true, stnk.IsInvoiceValid);
    }

    [Fact]
    public void Receive_Throws_WhenNotProcess()
    {
        var stnk = DefaultStnk();

        // Still in RECEIVE_FAKTUR — cannot receive
        Assert.Throws<DomainException>(() =>
            stnk.Receive(
                DateOnly.FromDateTime(DateTime.Today), "B1234XY", BiroId, "STNK-001",
                500_000m, 50_000m, 0m, "INV-001", null, 100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
                true, "admin"));
    }

    [Fact]
    public void HandOver_SetsStatusHandoverStnk()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        stnk.Receive(
            DateOnly.FromDateTime(DateTime.Today), "B1234XY", BiroId, "STNK-001",
            500_000m, 50_000m, 0m, "INV-001", null, 100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
            true, "admin");
        stnk.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin");

        Assert.Equal(StnkStatus.HANDOVER_STNK, stnk.Status);
        Assert.Equal("Budi Santoso", stnk.StnkReceiver);
    }

    [Fact]
    public void HandOver_Throws_WhenNotReceive()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");

        // in PROCESS — cannot hand over
        Assert.Throws<DomainException>(() =>
            stnk.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi", "admin"));
    }

    [Fact]
    public void Reconstitute_RestoresFullState()
    {
        var stnk = DefaultStnk();
        stnk.Process(DateOnly.FromDateTime(DateTime.Today), BiroId, "INV-001", "admin");
        stnk.Receive(
            DateOnly.FromDateTime(DateTime.Today), "B1234XY", BiroId, "STNK-001",
            500_000m, 50_000m, 0m, "INV-001", "DKI", 100_000m, 50_000m, 25_000m, 10_000m, 20_000m, 5_000m,
            true, "admin");
        stnk.HandOver(DateOnly.FromDateTime(DateTime.Today), "Budi Santoso", "admin");

        var reconstituted = Domain.Stnk.Stnk.Reconstitute(stnk.DomainEvents);

        Assert.Equal(StnkStatus.HANDOVER_STNK, reconstituted.Status);
        Assert.Equal(stnk.RegistrasiPenjualanId, reconstituted.RegistrasiPenjualanId);
        Assert.Equal(stnk.PlateNumber, reconstituted.PlateNumber);
        Assert.Equal(stnk.StnkReceiver, reconstituted.StnkReceiver);
        Assert.Equal(4, reconstituted.Version);
    }
}

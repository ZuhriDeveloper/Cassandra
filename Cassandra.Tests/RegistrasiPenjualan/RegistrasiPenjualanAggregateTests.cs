using Cassandra.Domain.Common;
using Cassandra.Domain.RegistrasiPenjualan;

namespace Cassandra.Tests.RegistrasiPenjualan;

public class RegistrasiPenjualanAggregateTests
{
    private static readonly Guid DealerId = Guid.NewGuid();

    private static Domain.RegistrasiPenjualan.RegistrasiPenjualan CreateSample(
        bool isApproved = false,
        string metode = "CASH",
        decimal discount = 0m,
        decimal total = 10_000_000m,
        decimal dp = 0m)
        => Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
            "PJ-001",
            DateOnly.FromDateTime(DateTime.Today),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            metode,
            TipePenjualanConstants.DIRECT,
            "M001",
            "R001",
            "Customer A",
            "Jl. Merdeka 1",
            "08123456789",
            null, null,
            0m, 0m,
            discount, 0m, 0m,
            total,
            0m, dp, 0m, 0m,
            null, null,
            "CODE-TM",
            "Merah",
            "STK-001",
            null,
            "",
            isApproved,
            "admin",
            DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_SetsInitialState()
    {
        var reg = CreateSample();

        Assert.Equal("PJ-001", reg.NoPenjualan);
        Assert.False(reg.IsApproved);
        Assert.False(reg.IsSent);
        Assert.False(reg.IsVoid);
        Assert.False(reg.EnableToVoid);
        Assert.Equal(DealerId, reg.DealerId);
        Assert.Equal(1, reg.Version);
    }

    [Fact]
    public void Create_AutoApproved_WhenIsApprovedTrue()
    {
        var reg = CreateSample(isApproved: true);
        Assert.True(reg.IsApproved);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Create_Throws_WhenNoPenjualanEmpty(string noPenjualan)
    {
        Assert.Throws<DomainException>(() =>
            Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
                noPenjualan, DateOnly.FromDateTime(DateTime.Today),
                Guid.NewGuid(), Guid.NewGuid(), null,
                "CASH", "DIRECT", "M001", "R001",
                "Customer", "Addr", "0812", null, null,
                0m, 0m, 0m, 0m, 0m, 10_000_000m,
                0m, 0m, 0m, 0m, null, null,
                "CODE", "WARNA", "STK", null, "", false, "admin", DealerId));
    }

    [Fact]
    public void Create_Throws_WhenTotalZero()
    {
        Assert.Throws<DomainException>(() =>
            Domain.RegistrasiPenjualan.RegistrasiPenjualan.Create(
                "PJ-001", DateOnly.FromDateTime(DateTime.Today),
                Guid.NewGuid(), Guid.NewGuid(), null,
                "CASH", "DIRECT", "M001", "R001",
                "Customer", "Addr", "0812", null, null,
                0m, 0m, 0m, 0m, 0m, 0m,
                0m, 0m, 0m, 0m, null, null,
                "CODE", "WARNA", "STK", null, "", false, "admin", DealerId));
    }

    // ── Approve ───────────────────────────────────────────────────────────────

    [Fact]
    public void Approve_SetsIsApprovedTrue()
    {
        var reg = CreateSample();
        reg.Approve(500_000m, "manager");

        Assert.True(reg.IsApproved);
        Assert.Equal(500_000m, reg.ApprovedDiscount);
    }

    [Fact]
    public void Approve_Throws_WhenAlreadyApproved()
    {
        var reg = CreateSample(isApproved: true);
        Assert.Throws<DomainException>(() => reg.Approve(0m, "manager"));
    }

    [Fact]
    public void Approve_Throws_WhenVoided()
    {
        var reg = CreateSample();
        reg.SetEnableToVoid(true, "admin");
        reg.Void("admin");
        Assert.Throws<DomainException>(() => reg.Approve(0m, "manager"));
    }

    // ── MarkAsSent ────────────────────────────────────────────────────────────

    [Fact]
    public void MarkAsSent_SetsIsSentTrue()
    {
        var reg = CreateSample(isApproved: true);
        reg.MarkAsSent("driver");
        Assert.True(reg.IsSent);
    }

    [Fact]
    public void MarkAsSent_Throws_WhenNotApproved()
    {
        var reg = CreateSample();
        Assert.Throws<DomainException>(() => reg.MarkAsSent("driver"));
    }

    [Fact]
    public void MarkAsSent_Throws_WhenAlreadySent()
    {
        var reg = CreateSample(isApproved: true);
        reg.MarkAsSent("driver");
        Assert.Throws<DomainException>(() => reg.MarkAsSent("driver"));
    }

    // ── Void ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Void_SetsIsVoidTrue()
    {
        var reg = CreateSample();
        reg.SetEnableToVoid(true, "admin");
        reg.Void("admin");
        Assert.True(reg.IsVoid);
    }

    [Fact]
    public void Void_Throws_WhenNotEnableToVoid()
    {
        var reg = CreateSample();
        Assert.Throws<DomainException>(() => reg.Void("admin"));
    }

    [Fact]
    public void Void_Throws_WhenAlreadyVoided()
    {
        var reg = CreateSample();
        reg.SetEnableToVoid(true, "admin");
        reg.Void("admin");
        Assert.Throws<DomainException>(() => reg.Void("admin"));
    }

    // ── SetEnableToVoid ───────────────────────────────────────────────────────

    [Fact]
    public void SetEnableToVoid_UpdatesField()
    {
        var reg = CreateSample();
        reg.SetEnableToVoid(true, "admin");
        Assert.True(reg.EnableToVoid);

        reg.SetEnableToVoid(false, "admin");
        Assert.False(reg.EnableToVoid);
    }

    [Fact]
    public void SetEnableToVoid_Throws_WhenVoided()
    {
        var reg = CreateSample();
        reg.SetEnableToVoid(true, "admin");
        reg.Void("admin");
        Assert.Throws<DomainException>(() => reg.SetEnableToVoid(true, "admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_RestoresState()
    {
        var original = CreateSample(isApproved: true);
        original.MarkAsSent("driver");

        var events = original.DomainEvents;
        var reconstituted = Domain.RegistrasiPenjualan.RegistrasiPenjualan.Reconstitute(events);

        Assert.Equal(original.NoPenjualan, reconstituted.NoPenjualan);
        Assert.Equal(original.IsApproved,  reconstituted.IsApproved);
        Assert.Equal(original.IsSent,      reconstituted.IsSent);
        Assert.Equal(original.DealerId,    reconstituted.DealerId);
    }
}

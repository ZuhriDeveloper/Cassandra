using Cassandra.Domain.Common;
using Cassandra.Domain.Mediator;
using Cassandra.Domain.Mediator.Events;

namespace Cassandra.Tests.Mediator;

public class MediatorTests
{
    private static readonly Guid DealerId    = Guid.NewGuid();
    private static readonly Guid KaryawanId  = Guid.NewGuid();
    private static readonly Guid AltKaryawan = Guid.NewGuid();

    private static Domain.Mediator.Mediator MakeMediator(
        string  name       = "Agen Utama",
        Guid?   karyawanId = null,
        string  address    = "Jl. Agen No. 1",
        decimal limit      = 30_000_000m) =>
        Domain.Mediator.Mediator.Create(
            name, karyawanId ?? KaryawanId, address, limit, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_raises_MediatorCreated_and_sets_state()
    {
        var mediator = MakeMediator();

        Assert.Single(mediator.DomainEvents);
        var evt = Assert.IsType<MediatorCreated>(mediator.DomainEvents[0]);

        Assert.Equal("Agen Utama", evt.Name);
        Assert.Equal(KaryawanId, evt.KaryawanId);
        Assert.Equal(30_000_000m, evt.Limit);
        Assert.Equal(DealerId, evt.DealerId);

        Assert.Equal("Agen Utama", mediator.Name);
        Assert.True(mediator.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_name(string name)
    {
        Assert.Throws<DomainException>(() => MakeMediator(name: name));
    }

    [Fact]
    public void Create_rejects_empty_karyawan()
    {
        Assert.Throws<DomainException>(() => MakeMediator(karyawanId: Guid.Empty));
    }

    [Fact]
    public void Create_rejects_negative_limit()
    {
        Assert.Throws<DomainException>(() => MakeMediator(limit: -1m));
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_changes_fields_and_raises_event()
    {
        var mediator = MakeMediator();
        mediator.ClearDomainEvents();

        mediator.Update("Agen Baru", AltKaryawan, "Jl. Baru 99", "admin");

        var evt = Assert.IsType<MediatorUpdated>(Assert.Single(mediator.DomainEvents));
        Assert.Equal("Agen Baru", evt.Name);
        Assert.Equal(AltKaryawan, evt.KaryawanId);
        Assert.Equal("Jl. Baru 99", evt.Address);
    }

    [Fact]
    public void Update_to_same_values_is_a_noop()
    {
        var mediator = MakeMediator();
        mediator.ClearDomainEvents();

        mediator.Update("Agen Utama", KaryawanId, "Jl. Agen No. 1", "admin");

        Assert.Empty(mediator.DomainEvents);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_then_activate_toggles_status_with_guards()
    {
        var mediator = MakeMediator();
        mediator.ClearDomainEvents();

        mediator.Deactivate("admin");
        Assert.False(mediator.IsActive);
        Assert.Throws<DomainException>(() => mediator.Deactivate("admin"));

        mediator.ClearDomainEvents();
        mediator.Activate("admin");
        Assert.True(mediator.IsActive);
        Assert.Throws<DomainException>(() => mediator.Activate("admin"));
    }

    // ── SetLimit ──────────────────────────────────────────────────────────────

    [Fact]
    public void SetLimit_updates_limit_and_raises_event()
    {
        var mediator = MakeMediator();
        mediator.ClearDomainEvents();

        mediator.SetLimit(80_000_000m, "admin");

        var evt = Assert.IsType<MediatorLimitSet>(Assert.Single(mediator.DomainEvents));
        Assert.Equal(80_000_000m, evt.Limit);
        Assert.Equal(80_000_000m, mediator.Limit);
    }

    [Fact]
    public void SetLimit_negative_throws()
    {
        var mediator = MakeMediator();
        Assert.Throws<DomainException>(() => mediator.SetLimit(-1m, "admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_replays_events_to_same_state()
    {
        var mediator = MakeMediator();
        mediator.Update("Agen B", AltKaryawan, "Jl. B 2", "admin");
        mediator.Deactivate("admin");
        mediator.SetLimit(100_000_000m, "admin");

        var replayed = Domain.Mediator.Mediator.Reconstitute(mediator.DomainEvents);

        Assert.Equal(mediator.Id,         replayed.Id);
        Assert.Equal(mediator.Name,       replayed.Name);
        Assert.Equal(mediator.KaryawanId, replayed.KaryawanId);
        Assert.Equal(mediator.Address,    replayed.Address);
        Assert.Equal(mediator.Limit,      replayed.Limit);
        Assert.Equal(mediator.IsActive,   replayed.IsActive);
    }
}

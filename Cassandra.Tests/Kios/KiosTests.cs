using Cassandra.Domain.Common;
using Cassandra.Domain.Kios;
using Cassandra.Domain.Kios.Events;

namespace Cassandra.Tests.Kios;

public class KiosTests
{
    private static readonly Guid   DealerId      = Guid.NewGuid();
    private static readonly Guid   PicKaryawanId = Guid.NewGuid();
    private static readonly Guid   AltPicId      = Guid.NewGuid();

    private static Domain.Kios.Kios MakeKios(
        string  code   = "K001",
        string  name   = "Kios Utama",
        string  phone  = "021-12345",
        string? fax    = null,
        string  addr   = "Jl. Merdeka 1",
        decimal limit  = 50_000_000m) =>
        Domain.Kios.Kios.Create(code, name, phone, fax, addr, PicKaryawanId, limit, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_raises_KiosCreated_and_sets_state()
    {
        var kios = MakeKios();

        Assert.Single(kios.DomainEvents);
        var evt = Assert.IsType<KiosCreated>(kios.DomainEvents[0]);

        Assert.Equal("K001", evt.Code);
        Assert.Equal("Kios Utama", evt.Name);
        Assert.Equal("021-12345", evt.Phone);
        Assert.Equal(50_000_000m, evt.Limit);
        Assert.Equal(DealerId, evt.DealerId);

        Assert.Equal("K001", kios.Code);
        Assert.True(kios.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_code(string code)
    {
        Assert.Throws<DomainException>(() => MakeKios(code: code));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_name(string name)
    {
        Assert.Throws<DomainException>(() => MakeKios(name: name));
    }

    [Fact]
    public void Create_rejects_negative_limit()
    {
        Assert.Throws<DomainException>(() => MakeKios(limit: -1m));
    }

    [Fact]
    public void Create_normalises_code_to_uppercase()
    {
        var kios = MakeKios(code: "k001");
        Assert.Equal("K001", kios.Code);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_changes_fields_and_raises_event()
    {
        var kios = MakeKios();
        kios.ClearDomainEvents();

        kios.Update("Kios Baru", "022-99999", "022-88888", "Jl. Baru 2", AltPicId, "admin");

        var evt = Assert.IsType<KiosUpdated>(Assert.Single(kios.DomainEvents));
        Assert.Equal("Kios Baru", evt.Name);
        Assert.Equal("022-99999", evt.Phone);
        Assert.Equal("022-88888", evt.Fax);
        Assert.Equal(AltPicId, evt.PicKaryawanId);
    }

    [Fact]
    public void Update_to_same_values_is_a_noop()
    {
        var kios = MakeKios(phone: "021-12345");
        kios.ClearDomainEvents();

        kios.Update("Kios Utama", "021-12345", null, "Jl. Merdeka 1", PicKaryawanId, "admin");

        Assert.Empty(kios.DomainEvents);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_then_activate_toggles_status_with_guards()
    {
        var kios = MakeKios();
        kios.ClearDomainEvents();

        kios.Deactivate("admin");
        Assert.False(kios.IsActive);
        Assert.Throws<DomainException>(() => kios.Deactivate("admin"));

        kios.ClearDomainEvents();
        kios.Activate("admin");
        Assert.True(kios.IsActive);
        Assert.Throws<DomainException>(() => kios.Activate("admin"));
    }

    // ── SetLimit ──────────────────────────────────────────────────────────────

    [Fact]
    public void SetLimit_updates_limit_and_raises_event()
    {
        var kios = MakeKios();
        kios.ClearDomainEvents();

        kios.SetLimit(100_000_000m, "admin");

        var evt = Assert.IsType<KiosLimitSet>(Assert.Single(kios.DomainEvents));
        Assert.Equal(100_000_000m, evt.Limit);
        Assert.Equal(100_000_000m, kios.Limit);
    }

    [Fact]
    public void SetLimit_negative_throws()
    {
        var kios = MakeKios();
        Assert.Throws<DomainException>(() => kios.SetLimit(-1m, "admin"));
    }

    // ── Reconstitute ──────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_replays_events_to_same_state()
    {
        var kios = MakeKios();
        kios.Update("Kios B", "022-00001", null, "Jl. B 1", AltPicId, "admin");
        kios.Deactivate("admin");
        kios.SetLimit(200_000_000m, "admin");

        var replayed = Domain.Kios.Kios.Reconstitute(kios.DomainEvents);

        Assert.Equal(kios.Id,            replayed.Id);
        Assert.Equal(kios.Code,          replayed.Code);
        Assert.Equal(kios.Name,          replayed.Name);
        Assert.Equal(kios.Phone,         replayed.Phone);
        Assert.Equal(kios.Fax,           replayed.Fax);
        Assert.Equal(kios.Address,       replayed.Address);
        Assert.Equal(kios.PicKaryawanId, replayed.PicKaryawanId);
        Assert.Equal(kios.Limit,         replayed.Limit);
        Assert.Equal(kios.IsActive,      replayed.IsActive);
    }
}

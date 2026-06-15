using Cassandra.Domain.Common;
using Cassandra.Domain.Karyawan;
using Cassandra.Domain.Karyawan.Events;

namespace Cassandra.Tests.Karyawan;

public class KaryawanTests
{
    private static readonly Guid     DealerId  = Guid.NewGuid();
    private static readonly Guid     JabatanId = Guid.NewGuid();
    private static readonly DateOnly HireDate  = new(2024, 1, 15);

    private static Domain.Karyawan.Karyawan DefaultKaryawan(string name = "Budi Santoso") =>
        Domain.Karyawan.Karyawan.Create(
            name, "budi@cassandra.local", "3201234567890001",
            Gender.Male, HireDate,
            "08123456789", null, "Jl. Merdeka No. 1",
            0m, JabatanId, "admin", DealerId);

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_raises_KaryawanCreated_and_sets_state()
    {
        var k = DefaultKaryawan("  Budi Santoso  ");

        Assert.Equal("Budi Santoso", k.Name);
        Assert.Equal("budi@cassandra.local", k.Email);
        Assert.Equal("3201234567890001", k.KtpNumber);
        Assert.Equal(Gender.Male, k.Gender);
        Assert.Equal(HireDate, k.HireDate);
        Assert.Null(k.ResignDate);
        Assert.Equal(DealerId, k.DealerId);
        Assert.Equal(JabatanId, k.JabatanId);
        Assert.True(k.IsActive);
        Assert.Equal(1, k.Version);

        var evt = Assert.IsType<KaryawanCreated>(Assert.Single(k.DomainEvents));
        Assert.Equal("Budi Santoso", evt.Name);
        Assert.Equal(DealerId, evt.DealerId);
        Assert.Equal(k.Id, evt.KaryawanId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_empty_name(string name)
    {
        Assert.Throws<DomainException>(() => DefaultKaryawan(name));
    }

    [Fact]
    public void Create_rejects_negative_sales_limit()
    {
        Assert.Throws<DomainException>(() =>
            Domain.Karyawan.Karyawan.Create(
                "Budi", "budi@x.com", "KTP", Gender.Male, HireDate,
                "08123", null, "Jl.", -1m, JabatanId, "admin", DealerId));
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public void Update_changes_fields_and_raises_event()
    {
        var k = DefaultKaryawan();
        k.ClearDomainEvents();

        k.Update("Siti Rahayu", "siti@cassandra.local", "081999", null, "Jl. Baru", JabatanId, "admin");

        Assert.Equal("Siti Rahayu", k.Name);
        Assert.Equal("siti@cassandra.local", k.Email);
        Assert.IsType<KaryawanUpdated>(Assert.Single(k.DomainEvents));
    }

    [Fact]
    public void Update_to_same_values_is_a_noop()
    {
        var k = DefaultKaryawan();
        k.ClearDomainEvents();

        k.Update("Budi Santoso", "budi@cassandra.local", "08123456789", null, "Jl. Merdeka No. 1", JabatanId, "admin");

        Assert.Empty(k.DomainEvents);
    }

    // ── Activate / Deactivate ─────────────────────────────────────────────────

    [Fact]
    public void Deactivate_then_activate_toggles_status_with_guards()
    {
        var k = DefaultKaryawan();

        k.Deactivate("admin");
        Assert.False(k.IsActive);
        Assert.Throws<DomainException>(() => k.Deactivate("admin"));

        k.Activate("admin");
        Assert.True(k.IsActive);
        Assert.Throws<DomainException>(() => k.Activate("admin"));
    }

    // ── Resign ────────────────────────────────────────────────────────────────

    [Fact]
    public void RecordResign_sets_resign_date_and_raises_event()
    {
        var k = DefaultKaryawan();
        k.ClearDomainEvents();
        var resignDate = new DateOnly(2025, 6, 30);

        k.RecordResign(resignDate, "admin");

        Assert.Equal(resignDate, k.ResignDate);
        var evt = Assert.IsType<KaryawanResigned>(Assert.Single(k.DomainEvents));
        Assert.Equal(resignDate, evt.ResignDate);
    }

    [Fact]
    public void RecordResign_twice_throws()
    {
        var k = DefaultKaryawan();
        k.RecordResign(new DateOnly(2025, 1, 1), "admin");

        Assert.Throws<DomainException>(() => k.RecordResign(new DateOnly(2025, 6, 30), "admin"));
    }

    // ── SetLimit ──────────────────────────────────────────────────────────────

    [Fact]
    public void SetLimit_updates_sales_limit_and_raises_event()
    {
        var k = DefaultKaryawan();
        k.ClearDomainEvents();

        k.SetLimit(50_000_000m, "admin");

        Assert.Equal(50_000_000m, k.SalesLimit);
        var evt = Assert.IsType<KaryawanLimitSet>(Assert.Single(k.DomainEvents));
        Assert.Equal(50_000_000m, evt.SalesLimit);
    }

    [Fact]
    public void SetLimit_negative_throws()
    {
        var k = DefaultKaryawan();
        Assert.Throws<DomainException>(() => k.SetLimit(-1m, "admin"));
    }

    // ── Reconstitute ─────────────────────────────────────────────────────────

    [Fact]
    public void Reconstitute_replays_events_to_same_state()
    {
        var original = DefaultKaryawan();
        original.Update("Siti", "siti@x.com", "08199", null, "Jl. Baru", JabatanId, "admin");
        original.SetLimit(10_000_000m, "admin");
        original.Deactivate("admin");

        IEnumerable<IDomainEvent> events = original.DomainEvents.ToList();
        var rebuilt = Domain.Karyawan.Karyawan.Reconstitute(events);

        Assert.Equal(original.Id, rebuilt.Id);
        Assert.Equal("Siti", rebuilt.Name);
        Assert.Equal(10_000_000m, rebuilt.SalesLimit);
        Assert.False(rebuilt.IsActive);
        Assert.Equal(DealerId, rebuilt.DealerId);
        Assert.Equal(original.Version, rebuilt.Version);
        Assert.Empty(rebuilt.DomainEvents);
    }
}

using Cassandra.Domain.Common;
using Cassandra.Domain.Dealers.Events;

namespace Cassandra.Domain.Dealers;

/// <summary>
/// A dealer (motorcycle dealership) — the multi-tenant root. Every transactional record in
/// the system is scoped to a <see cref="DealerId"/>. The dealer aggregate itself is the
/// tenant boundary; its own events are persisted under its own id.
/// </summary>
public class Dealer : AggregateRoot<DealerId>
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Dealer() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Dealer Register(string name, string code, string registeredBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Dealer name cannot be empty.");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Dealer code cannot be empty.");

        var dealer = new Dealer();
        dealer.Raise(new DealerRegistered(
            DealerId.New(),
            name.Trim(),
            code.Trim(),
            registeredBy,
            DateTime.UtcNow));
        return dealer;
    }

    public static Dealer Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var dealer = new Dealer();
        dealer.Load(events);
        return dealer;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Rename(string name, string renamedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Dealer name cannot be empty.");

        if (Name != name.Trim())
            Raise(new DealerRenamed(Id, name.Trim(), renamedBy, DateTime.UtcNow));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (!IsActive)
            throw new DomainException("Dealer is already inactive.");

        Raise(new DealerDeactivated(Id, deactivatedBy, DateTime.UtcNow));
    }

    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new DomainException("Dealer is already active.");

        Raise(new DealerActivated(Id, activatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DealerRegistered e:
                Id = e.DealerId;
                Name = e.Name;
                Code = e.Code;
                IsActive = true;
                break;

            case DealerRenamed e:
                Name = e.Name;
                break;

            case DealerDeactivated:
                IsActive = false;
                break;

            case DealerActivated:
                IsActive = true;
                break;
        }
    }
}

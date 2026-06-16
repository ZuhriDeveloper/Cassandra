using Cassandra.Domain.Common;
using Cassandra.Domain.Ledger.Events;

namespace Cassandra.Domain.Ledger;

public class Ledger : AggregateRoot<LedgerId>
{
    public Guid DealerId { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Ledger() { }

    public static Ledger Create(string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama ledger tidak boleh kosong.");

        var ledger = new Ledger();
        ledger.Raise(new LedgerCreated(LedgerId.New(), dealerId, name.Trim(), createdBy, DateTime.UtcNow));
        return ledger;
    }

    public static Ledger Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var ledger = new Ledger();
        ledger.Load(events);
        return ledger;
    }

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama ledger tidak boleh kosong.");

        var trimmed = name.Trim();
        if (Name == trimmed) return;

        Raise(new LedgerUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Ledger sudah aktif.");

        Raise(new LedgerActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Ledger sudah tidak aktif.");

        Raise(new LedgerDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case LedgerCreated e:
                Id = e.LedgerId;
                DealerId = e.DealerId;
                Name = e.Name;
                IsActive = true;
                break;

            case LedgerUpdated e:
                Name = e.Name;
                break;

            case LedgerActivated:
                IsActive = true;
                break;

            case LedgerDeactivated:
                IsActive = false;
                break;
        }
    }
}

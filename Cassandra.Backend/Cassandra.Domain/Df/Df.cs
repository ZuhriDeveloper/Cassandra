using Cassandra.Domain.Common;
using Cassandra.Domain.Df.Events;

namespace Cassandra.Domain.Df;

public class Df : AggregateRoot<DfId>
{
    public Guid DealerId { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Interest { get; private set; }
    public DateOnly StartDate { get; private set; }

    private Df() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Df Create(decimal discount, decimal interest, DateOnly startDate, string createdBy, Guid dealerId)
    {
        var df = new Df();
        df.Raise(new DfSet(DfId.New(), dealerId, discount, interest, startDate, createdBy, DateTime.UtcNow));
        return df;
    }

    public static Df Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var df = new Df();
        df.Load(events);
        return df;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Set(decimal discount, decimal interest, DateOnly startDate, string updatedBy)
        => Raise(new DfSet(Id, DealerId, discount, interest, startDate, updatedBy, DateTime.UtcNow));

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DfSet e:
                Id = e.DfId;
                DealerId = e.DealerId;
                Discount = e.Discount;
                Interest = e.Interest;
                StartDate = e.StartDate;
                break;
        }
    }
}

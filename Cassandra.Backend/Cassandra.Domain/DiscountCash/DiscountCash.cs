using Cassandra.Domain.Common;
using Cassandra.Domain.DiscountCash.Events;

namespace Cassandra.Domain.DiscountCash;

public class DiscountCash : AggregateRoot<DiscountCashId>
{
    public Guid DealerId { get; private set; }
    public Guid TipeMotorId { get; private set; }
    public decimal DirectDiscount { get; private set; }
    public decimal ChannelDiscount { get; private set; }
    public bool IsActive { get; private set; }

    private DiscountCash() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static DiscountCash Create(
        Guid tipeMotorId, decimal directDiscount, decimal channelDiscount,
        string createdBy, Guid dealerId)
    {
        if (tipeMotorId == Guid.Empty)
            throw new DomainException("Tipe motor harus dipilih.");
        if (directDiscount < 0)
            throw new DomainException("Direct discount tidak boleh negatif.");
        if (channelDiscount < 0)
            throw new DomainException("Channel discount tidak boleh negatif.");

        var dc = new DiscountCash();
        dc.Raise(new DiscountCashCreated(
            DiscountCashId.New(),
            dealerId,
            tipeMotorId,
            directDiscount,
            channelDiscount,
            createdBy,
            DateTime.UtcNow));
        return dc;
    }

    public static DiscountCash Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var dc = new DiscountCash();
        dc.Load(events);
        return dc;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(decimal directDiscount, decimal channelDiscount, string updatedBy)
    {
        if (directDiscount < 0)
            throw new DomainException("Direct discount tidak boleh negatif.");
        if (channelDiscount < 0)
            throw new DomainException("Channel discount tidak boleh negatif.");

        if (DirectDiscount == directDiscount && ChannelDiscount == channelDiscount) return;

        Raise(new DiscountCashUpdated(Id, directDiscount, channelDiscount, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Discount cash sudah aktif.");

        Raise(new DiscountCashActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Discount cash sudah tidak aktif.");

        Raise(new DiscountCashDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case DiscountCashCreated e:
                Id = e.DiscountCashId;
                DealerId = e.DealerId;
                TipeMotorId = e.TipeMotorId;
                DirectDiscount = e.DirectDiscount;
                ChannelDiscount = e.ChannelDiscount;
                IsActive = true;
                break;

            case DiscountCashUpdated e:
                DirectDiscount = e.DirectDiscount;
                ChannelDiscount = e.ChannelDiscount;
                break;

            case DiscountCashActivated:
                IsActive = true;
                break;

            case DiscountCashDeactivated:
                IsActive = false;
                break;
        }
    }
}

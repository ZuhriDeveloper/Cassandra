using Cassandra.Domain.Bpkb.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.Bpkb;

public class Bpkb : AggregateRoot<BpkbId>
{
    public Guid     DealerId              { get; private set; }
    public Guid     RegistrasiPenjualanId { get; private set; }
    public Guid     StnkId               { get; private set; }
    public string   Status               { get; private set; } = default!;
    public DateOnly RequestDate          { get; private set; }
    public string?  BpkbNumber           { get; private set; }
    public string?  BookNumber           { get; private set; }
    public DateOnly? ReceiveDate         { get; private set; }
    public DateOnly? HandoverDate        { get; private set; }
    public string?  Receiver             { get; private set; }

    private Bpkb() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Bpkb Create(
        Guid     registrasiPenjualanId,
        Guid     stnkId,
        DateOnly requestDate,
        string   createdBy,
        Guid     dealerId)
    {
        var bpkb = new Bpkb();
        bpkb.Raise(new BpkbCreated(
            BpkbId.New(),
            dealerId,
            registrasiPenjualanId,
            stnkId,
            requestDate,
            createdBy,
            DateTime.UtcNow));
        return bpkb;
    }

    public static Bpkb Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var bpkb = new Bpkb();
        bpkb.Load(events);
        return bpkb;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Receive(string bpkbNumber, string bookNumber, DateOnly receiveDate, string updatedBy)
    {
        if (Status != BpkbStatus.REQUEST)
            throw new DomainException($"BPKB tidak dapat diterima karena statusnya '{Status}', bukan '{BpkbStatus.REQUEST}'.");

        Raise(new BpkbReceived(Id, bpkbNumber, bookNumber, receiveDate, updatedBy, DateTime.UtcNow));
    }

    public void HandOver(DateOnly handoverDate, string receiver, string updatedBy)
    {
        if (Status != BpkbStatus.RECEIVE)
            throw new DomainException($"BPKB tidak dapat diserahkan karena statusnya '{Status}', bukan '{BpkbStatus.RECEIVE}'.");

        Raise(new BpkbHandedOver(Id, handoverDate, receiver, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BpkbCreated e:
                Id                    = e.BpkbId;
                DealerId              = e.DealerId;
                RegistrasiPenjualanId = e.RegistrasiPenjualanId;
                StnkId                = e.StnkId;
                RequestDate           = e.RequestDate;
                Status                = BpkbStatus.REQUEST;
                break;

            case BpkbReceived e:
                BpkbNumber  = e.BpkbNumber;
                BookNumber  = e.BookNumber;
                ReceiveDate = e.ReceiveDate;
                Status      = BpkbStatus.RECEIVE;
                break;

            case BpkbHandedOver e:
                HandoverDate = e.HandoverDate;
                Receiver     = e.Receiver;
                Status       = BpkbStatus.HANDOVER;
                break;
        }
    }
}

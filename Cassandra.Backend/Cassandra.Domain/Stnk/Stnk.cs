using Cassandra.Domain.Common;
using Cassandra.Domain.Stnk.Events;

namespace Cassandra.Domain.Stnk;

public class Stnk : AggregateRoot<StnkId>
{
    public Guid     DealerId              { get; private set; }
    public Guid     RegistrasiPenjualanId { get; private set; }
    public DateOnly FakturDate            { get; private set; }
    public string   FakturName            { get; private set; } = default!;
    public string   FakturAddress         { get; private set; } = default!;
    public string   Status                { get; private set; } = default!;

    public DateOnly? ProcessDate    { get; private set; }
    public Guid?     BiroId         { get; private set; }
    public string?   InvoiceNumber  { get; private set; }
    public string?   PlateNumber    { get; private set; }
    public string?   StnkNumber     { get; private set; }
    public decimal   StnkCost       { get; private set; }
    public decimal   ProgressiveCost { get; private set; }
    public decimal   NoticeCost     { get; private set; }
    public DateOnly? ReceiveDate    { get; private set; }
    public DateOnly? HandoverDate   { get; private set; }
    public string?   StnkReceiver   { get; private set; }
    public string?   Region         { get; private set; }
    public decimal   BbnCost        { get; private set; }
    public decimal   PnbpCost       { get; private set; }
    public decimal   AdminCost      { get; private set; }
    public decimal   OtherCost      { get; private set; }
    public decimal   ServiceCost    { get; private set; }
    public decimal   PphCost        { get; private set; }
    public bool?     IsInvoiceValid { get; private set; }

    private Stnk() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Stnk Create(
        Guid     registrasiPenjualanId,
        DateOnly fakturDate,
        string   fakturName,
        string   fakturAddress,
        string   createdBy,
        Guid     dealerId)
    {
        if (string.IsNullOrWhiteSpace(fakturName))
            throw new DomainException("Nama faktur tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(fakturAddress))
            throw new DomainException("Alamat faktur tidak boleh kosong.");

        var stnk = new Stnk();
        stnk.Raise(new StnkCreated(
            StnkId.New(),
            dealerId,
            registrasiPenjualanId,
            fakturDate,
            fakturName.Trim(),
            fakturAddress.Trim(),
            createdBy,
            DateTime.UtcNow));
        return stnk;
    }

    public static Stnk Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var stnk = new Stnk();
        stnk.Load(events);
        return stnk;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Process(DateOnly processDate, Guid biroId, string invoiceNumber, string updatedBy)
    {
        if (Status != StnkStatus.RECEIVE_FAKTUR)
            throw new DomainException($"STNK tidak dapat diproses karena statusnya '{Status}', bukan '{StnkStatus.RECEIVE_FAKTUR}'.");

        Raise(new StnkProcessed(Id, processDate, biroId, invoiceNumber, updatedBy, DateTime.UtcNow));
    }

    public void Receive(
        DateOnly receiveDate,
        string   plateNumber,
        Guid     biroId,
        string   stnkNumber,
        decimal  stnkCost,
        decimal  noticeCost,
        decimal  progressiveCost,
        string   invoiceNumber,
        string?  region,
        decimal  bbnCost,
        decimal  pnbpCost,
        decimal  adminCost,
        decimal  otherCost,
        decimal  serviceCost,
        decimal  pphCost,
        bool     isInvoiceValid,
        string   updatedBy)
    {
        if (Status != StnkStatus.PROCESS)
            throw new DomainException($"STNK tidak dapat diterima karena statusnya '{Status}', bukan '{StnkStatus.PROCESS}'.");

        Raise(new StnkReceived(
            Id, receiveDate, plateNumber, biroId, stnkNumber,
            stnkCost, noticeCost, progressiveCost, invoiceNumber, region,
            bbnCost, pnbpCost, adminCost, otherCost, serviceCost, pphCost,
            isInvoiceValid, updatedBy, DateTime.UtcNow));
    }

    public void HandOver(DateOnly handoverDate, string stnkReceiver, string updatedBy)
    {
        if (Status != StnkStatus.RECEIVE)
            throw new DomainException($"STNK tidak dapat diserahkan karena statusnya '{Status}', bukan '{StnkStatus.RECEIVE}'.");

        Raise(new StnkHandedOver(Id, handoverDate, stnkReceiver, updatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case StnkCreated e:
                Id                    = e.StnkId;
                DealerId              = e.DealerId;
                RegistrasiPenjualanId = e.RegistrasiPenjualanId;
                FakturDate            = e.FakturDate;
                FakturName            = e.FakturName;
                FakturAddress         = e.FakturAddress;
                Status                = StnkStatus.RECEIVE_FAKTUR;
                break;

            case StnkProcessed e:
                ProcessDate   = e.ProcessDate;
                BiroId        = e.BiroId;
                InvoiceNumber = e.InvoiceNumber;
                Status        = StnkStatus.PROCESS;
                break;

            case StnkReceived e:
                ReceiveDate     = e.ReceiveDate;
                PlateNumber     = e.PlateNumber;
                BiroId          = e.BiroId;
                StnkNumber      = e.StnkNumber;
                StnkCost        = e.StnkCost;
                NoticeCost      = e.NoticeCost;
                ProgressiveCost = e.ProgressiveCost;
                InvoiceNumber   = e.InvoiceNumber;
                Region          = e.Region;
                BbnCost         = e.BbnCost;
                PnbpCost        = e.PnbpCost;
                AdminCost       = e.AdminCost;
                OtherCost       = e.OtherCost;
                ServiceCost     = e.ServiceCost;
                PphCost         = e.PphCost;
                IsInvoiceValid  = e.IsInvoiceValid;
                Status          = StnkStatus.RECEIVE;
                break;

            case StnkHandedOver e:
                HandoverDate  = e.HandoverDate;
                StnkReceiver  = e.StnkReceiver;
                Status        = StnkStatus.HANDOVER_STNK;
                break;
        }
    }
}

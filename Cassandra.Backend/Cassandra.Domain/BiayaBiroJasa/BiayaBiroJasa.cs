using Cassandra.Domain.BiayaBiroJasa.Events;
using Cassandra.Domain.Common;

namespace Cassandra.Domain.BiayaBiroJasa;

public class BiayaBiroJasa : AggregateRoot<BiayaBiroJasaId>
{
    public Guid DealerId { get; private set; }
    public Guid SamsatId { get; private set; }
    public Guid BiroId { get; private set; }
    public bool IsActive { get; private set; }

    private List<BiayaBiroJasaItemValue> _items = new();
    public IReadOnlyList<BiayaBiroJasaItemValue> Items => _items.AsReadOnly();

    private BiayaBiroJasa() { }

    public static BiayaBiroJasa Create(Guid samsatId, Guid biroId, string createdBy, Guid dealerId)
    {
        if (samsatId == Guid.Empty)
            throw new DomainException("Samsat harus dipilih.");
        if (biroId == Guid.Empty)
            throw new DomainException("Biro harus dipilih.");

        var bbj = new BiayaBiroJasa();
        bbj.Raise(new BiayaBiroJasaCreated(BiayaBiroJasaId.New(), dealerId, samsatId, biroId, createdBy, DateTime.UtcNow));
        return bbj;
    }

    public static BiayaBiroJasa Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var bbj = new BiayaBiroJasa();
        bbj.Load(events);
        return bbj;
    }

    public void SetItems(IReadOnlyList<BiayaBiroJasaItemValue> items, string updatedBy)
    {
        foreach (var item in items)
        {
            if (item.BiayaStnk < 0)
                throw new DomainException("Biaya STNK tidak boleh negatif.");
            if (item.Notice < 0)
                throw new DomainException("Notice tidak boleh negatif.");
        }

        Raise(new BiayaBiroJasaItemsSet(Id, items, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Biaya biro jasa sudah aktif.");

        Raise(new BiayaBiroJasaActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Biaya biro jasa sudah tidak aktif.");

        Raise(new BiayaBiroJasaDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BiayaBiroJasaCreated e:
                Id = e.BiayaBiroJasaId;
                DealerId = e.DealerId;
                SamsatId = e.SamsatId;
                BiroId = e.BiroId;
                IsActive = true;
                break;

            case BiayaBiroJasaActivated:
                IsActive = true;
                break;

            case BiayaBiroJasaDeactivated:
                IsActive = false;
                break;

            case BiayaBiroJasaItemsSet e:
                _items = [.. e.Items];
                break;
        }
    }
}

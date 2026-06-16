using Cassandra.Domain.Common;
using Cassandra.Domain.Samsat.Events;

namespace Cassandra.Domain.Samsat;

public class Samsat : AggregateRoot<SamsatId>
{
    public Guid DealerId { get; private set; }
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private List<string> _cities = new();
    public IReadOnlyList<string> Cities => _cities.AsReadOnly();

    private Samsat() { }

    public static Samsat Create(string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama samsat tidak boleh kosong.");

        var samsat = new Samsat();
        samsat.Raise(new SamsatCreated(SamsatId.New(), dealerId, name.Trim(), createdBy, DateTime.UtcNow));
        return samsat;
    }

    public static Samsat Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var samsat = new Samsat();
        samsat.Load(events);
        return samsat;
    }

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama samsat tidak boleh kosong.");

        var trimmed = name.Trim();
        if (Name == trimmed) return;

        Raise(new SamsatUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Samsat sudah aktif.");

        Raise(new SamsatActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Samsat sudah tidak aktif.");

        Raise(new SamsatDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void SetCities(IReadOnlyList<string> cities, string updatedBy)
    {
        Raise(new SamsatCitiesSet(Id, cities, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SamsatCreated e:
                Id = e.SamsatId;
                DealerId = e.DealerId;
                Name = e.Name;
                IsActive = true;
                break;

            case SamsatUpdated e:
                Name = e.Name;
                break;

            case SamsatActivated:
                IsActive = true;
                break;

            case SamsatDeactivated:
                IsActive = false;
                break;

            case SamsatCitiesSet e:
                _cities = [.. e.Cities];
                break;
        }
    }
}

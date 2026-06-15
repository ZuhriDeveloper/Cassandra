using Cassandra.Domain.Common;
using Cassandra.Domain.Jabatan.Events;

namespace Cassandra.Domain.Jabatan;

public class Jabatan : AggregateRoot<JabatanId>
{
    public Guid DealerId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private Jabatan() { }

    // ── Factory ───────────────────────────────────────────────────────────────

    public static Jabatan Create(string name, string description, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama jabatan tidak boleh kosong.");

        var jabatan = new Jabatan();
        jabatan.Raise(new JabatanCreated(
            JabatanId.New(),
            dealerId,
            name.Trim(),
            description?.Trim() ?? string.Empty,
            createdBy,
            DateTime.UtcNow));
        return jabatan;
    }

    public static Jabatan Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var jabatan = new Jabatan();
        jabatan.Load(events);
        return jabatan;
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    public void Update(string name, string description, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama jabatan tidak boleh kosong.");

        var trimmedName = name.Trim();
        var trimmedDesc = description?.Trim() ?? string.Empty;

        if (Name != trimmedName || Description != trimmedDesc)
            Raise(new JabatanUpdated(Id, trimmedName, trimmedDesc, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string activatedBy)
    {
        if (IsActive)
            throw new DomainException("Jabatan sudah aktif.");

        Raise(new JabatanActivated(Id, activatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string deactivatedBy)
    {
        if (!IsActive)
            throw new DomainException("Jabatan sudah tidak aktif.");

        Raise(new JabatanDeactivated(Id, deactivatedBy, DateTime.UtcNow));
    }

    // ── Event Application ─────────────────────────────────────────────────────

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case JabatanCreated e:
                Id = e.JabatanId;
                DealerId = e.DealerId;
                Name = e.Name;
                Description = e.Description;
                IsActive = true;
                break;

            case JabatanUpdated e:
                Name = e.Name;
                Description = e.Description;
                break;

            case JabatanDeactivated:
                IsActive = false;
                break;

            case JabatanActivated:
                IsActive = true;
                break;
        }
    }
}

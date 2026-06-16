using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType.Events;

namespace Cassandra.Domain.ExpenseType;

public class ExpenseType : AggregateRoot<ExpenseTypeId>
{
    public Guid DealerId { get; private set; }
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    private ExpenseType() { }

    public static ExpenseType Create(string code, string name, string createdBy, Guid dealerId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kode tipe biaya tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama tipe biaya tidak boleh kosong.");

        var et = new ExpenseType();
        et.Raise(new ExpenseTypeCreated(
            ExpenseTypeId.New(),
            dealerId,
            code.Trim().ToUpper(),
            name.Trim(),
            createdBy,
            DateTime.UtcNow));
        return et;
    }

    public static ExpenseType Reconstitute(IEnumerable<IDomainEvent> events)
    {
        var et = new ExpenseType();
        et.Load(events);
        return et;
    }

    public void Update(string name, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nama tipe biaya tidak boleh kosong.");

        var trimmed = name.Trim();
        if (Name == trimmed) return;

        Raise(new ExpenseTypeUpdated(Id, trimmed, updatedBy, DateTime.UtcNow));
    }

    public void Activate(string updatedBy)
    {
        if (IsActive)
            throw new DomainException("Tipe biaya sudah aktif.");

        Raise(new ExpenseTypeActivated(Id, updatedBy, DateTime.UtcNow));
    }

    public void Deactivate(string updatedBy)
    {
        if (!IsActive)
            throw new DomainException("Tipe biaya sudah tidak aktif.");

        Raise(new ExpenseTypeDeactivated(Id, updatedBy, DateTime.UtcNow));
    }

    protected override void Apply(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ExpenseTypeCreated e:
                Id = e.ExpenseTypeId;
                DealerId = e.DealerId;
                Code = e.Code;
                Name = e.Name;
                IsActive = true;
                break;

            case ExpenseTypeUpdated e:
                Name = e.Name;
                break;

            case ExpenseTypeActivated:
                IsActive = true;
                break;

            case ExpenseTypeDeactivated:
                IsActive = false;
                break;
        }
    }
}

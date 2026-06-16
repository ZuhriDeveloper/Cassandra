using Cassandra.Application.Contracts.ExpenseType;
using Cassandra.Domain.Common;
using Cassandra.Domain.ExpenseType;
using Cassandra.Domain.ExpenseType.Events;
using Cassandra.Infrastructure.Persistence;
using Cassandra.Infrastructure.Persistence.EventStore;
using Cassandra.Infrastructure.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace Cassandra.Infrastructure.ExpenseType;

public class ExpenseTypeRepository(AppDbContext context) : IExpenseTypeRepository
{
    private const string AggregateType = "ExpenseType";

    public async Task<Domain.ExpenseType.ExpenseType?> GetByIdAsync(ExpenseTypeId id, CancellationToken ct = default)
    {
        var stored = await context.StoredEvents
            .Where(e => e.AggregateId == id.Value && e.AggregateType == AggregateType)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);

        if (stored.Count == 0) return null;

        return Domain.ExpenseType.ExpenseType.Reconstitute(
            stored.Select(e => EventSerializer.Deserialize(e.EventType, e.EventData)));
    }

    public async Task SaveAsync(Domain.ExpenseType.ExpenseType et, CancellationToken ct = default)
    {
        var newEvents = et.DomainEvents;
        if (newEvents.Count == 0) return;

        var baseVersion = et.Version - newEvents.Count;
        for (var i = 0; i < newEvents.Count; i++)
        {
            var evt = newEvents[i];
            context.StoredEvents.Add(new StoredEvent
            {
                DealerId      = et.DealerId,
                AggregateType = AggregateType,
                AggregateId   = et.Id.Value,
                Version       = baseVersion + i + 1,
                EventType     = EventTypeRegistry.GetName(evt),
                EventData     = EventSerializer.Serialize(evt),
                OccurredAt    = evt.OccurredAt,
            });
        }

        await UpdateProjectionAsync(et, newEvents, ct);
        await context.SaveChangesAsync(ct);
        et.ClearDomainEvents();
    }

    private async Task UpdateProjectionAsync(
        Domain.ExpenseType.ExpenseType et,
        IReadOnlyList<IDomainEvent> newEvents,
        CancellationToken ct)
    {
        var projection = await context.ExpenseTypeReadModels
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == et.Id.Value, ct);

        if (projection is null)
        {
            var created = newEvents.OfType<ExpenseTypeCreated>().First();
            projection = new ExpenseTypeReadModel
            {
                Id        = et.Id.Value,
                DealerId  = et.DealerId,
                Code      = created.Code,
                CreatedBy = created.CreatedBy,
                CreatedAt = created.OccurredAt,
            };
            context.ExpenseTypeReadModels.Add(projection);
        }

        if (newEvents.OfType<ExpenseTypeUpdated>().LastOrDefault() is { } upd)
        {
            projection.UpdatedBy = upd.UpdatedBy;
            projection.UpdatedAt = upd.OccurredAt;
        }

        if (newEvents.OfType<ExpenseTypeActivated>().LastOrDefault() is { } act)
        {
            projection.UpdatedBy = act.UpdatedBy;
            projection.UpdatedAt = act.OccurredAt;
        }

        if (newEvents.OfType<ExpenseTypeDeactivated>().LastOrDefault() is { } deact)
        {
            projection.UpdatedBy = deact.UpdatedBy;
            projection.UpdatedAt = deact.OccurredAt;
        }

        projection.Name     = et.Name;
        projection.IsActive = et.IsActive;
    }
}

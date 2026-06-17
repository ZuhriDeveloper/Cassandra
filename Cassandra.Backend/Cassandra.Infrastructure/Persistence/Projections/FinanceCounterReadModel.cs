namespace Cassandra.Infrastructure.Persistence.Projections;

public class FinanceCounterReadModel
{
    public Guid DealerId     { get; set; }
    public int  NextSequence { get; set; }
}

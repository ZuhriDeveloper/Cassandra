namespace Cassandra.Infrastructure.Dealers;

/// <summary>
/// Ambient dealer override for non-HTTP contexts (seeding, background jobs) where there is
/// no <c>dealerId</c> JWT claim. <see cref="CurrentDealer"/> consults this before the claim.
/// Usage:
/// <code>using (DealerScope.Begin(dealerId)) { /* dealer-scoped work */ }</code>
/// </summary>
public static class DealerScope
{
    private static readonly AsyncLocal<Guid?> Ambient = new();

    public static Guid? Current => Ambient.Value;

    public static IDisposable Begin(Guid dealerId)
    {
        var previous = Ambient.Value;
        Ambient.Value = dealerId;
        return new Restore(previous);
    }

    private sealed class Restore(Guid? previous) : IDisposable
    {
        public void Dispose() => Ambient.Value = previous;
    }
}

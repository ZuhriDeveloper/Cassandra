using Cassandra.Application.Authorization;
using Cassandra.Application.Contracts.Dealers;
using Microsoft.AspNetCore.Http;

namespace Cassandra.Infrastructure.Dealers;

/// <summary>
/// Resolves the current dealer from, in order: (1) an ambient <see cref="DealerScope"/>
/// override (seeders / background work), then (2) the <c>dealerId</c> claim on the
/// authenticated request. A platform <c>SuperAdmin</c> has neither and is unscoped.
/// </summary>
public sealed class CurrentDealer(IHttpContextAccessor accessor) : ICurrentDealer
{
    public Guid? DealerIdOrNull
    {
        get
        {
            if (DealerScope.Current is Guid ambient)
                return ambient;

            var raw = accessor.HttpContext?.User.FindFirst(DealerConstants.DealerClaimType)?.Value;
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public Guid DealerId =>
        DealerIdOrNull ?? throw new InvalidOperationException(
            "This operation requires a dealer-scoped user, but no dealer is in scope.");

    public bool IsSuperAdmin =>
        accessor.HttpContext?.User.IsInRole(Roles.SuperAdmin) ?? false;
}

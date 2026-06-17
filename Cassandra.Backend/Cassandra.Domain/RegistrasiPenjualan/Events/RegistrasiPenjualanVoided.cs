using Cassandra.Domain.Common;

namespace Cassandra.Domain.RegistrasiPenjualan.Events;

public record RegistrasiPenjualanVoided(
    RegistrasiPenjualanId Id,
    string   VoidedBy,
    DateTime OccurredAt) : IDomainEvent;

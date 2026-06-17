using Cassandra.Domain.Common;

namespace Cassandra.Domain.RegistrasiPenjualan.Events;

public record RegistrasiPenjualanSent(
    RegistrasiPenjualanId Id,
    string   SentBy,
    DateTime OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.RegistrasiPenjualan.Events;

public record RegistrasiPenjualanEnableToVoidSet(
    RegistrasiPenjualanId Id,
    bool     EnableToVoid,
    string   UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

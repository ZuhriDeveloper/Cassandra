using Cassandra.Domain.Common;

namespace Cassandra.Domain.RegistrasiPenjualan.Events;

public record RegistrasiPenjualanApproved(
    RegistrasiPenjualanId Id,
    decimal  ApprovedDiscount,
    string   ApprovedBy,
    DateTime OccurredAt) : IDomainEvent;

using Cassandra.Domain.Common;

namespace Cassandra.Domain.TipeMotor.Events;

public record TipeMotorUpdated(
    TipeMotorId TipeMotorId,
    Guid GrupTipeMotorId,
    string ShortName,
    string ProductCode,
    string WmsCode,
    string AhmCode,
    string EngineNumberFormat,
    string ChassisNumberFormat,
    decimal NettPrice,
    decimal OrJakarta,
    decimal OrTangerang,
    decimal BbnJakarta,
    decimal BbnTangerang,
    string UpdatedBy,
    DateTime OccurredAt) : IDomainEvent;

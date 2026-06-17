using Cassandra.Domain.Common;

namespace Cassandra.Domain.PengirimanMotor.Events;

public record PengirimanMotorCreated(
    PengirimanMotorId     Id,
    Guid                  DealerId,
    Guid                  RegistrasiPenjualanId,
    string                NoMesin,
    Guid                  Driver1Id,
    Guid?                 Driver2Id,
    DateOnly              DeliveryDate,
    string?               Zona,
    string                CreatedBy,
    DateTime              OccurredAt) : IDomainEvent;

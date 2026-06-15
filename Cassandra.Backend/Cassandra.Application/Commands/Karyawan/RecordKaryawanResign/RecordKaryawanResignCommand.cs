namespace Cassandra.Application.Commands.Karyawan.RecordKaryawanResign;

public record RecordKaryawanResignCommand(Guid Id, DateOnly ResignDate, string RecordedBy);

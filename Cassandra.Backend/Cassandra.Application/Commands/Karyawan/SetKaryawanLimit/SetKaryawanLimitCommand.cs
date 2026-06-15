namespace Cassandra.Application.Commands.Karyawan.SetKaryawanLimit;

public record SetKaryawanLimitCommand(Guid Id, decimal SalesLimit, string SetBy);

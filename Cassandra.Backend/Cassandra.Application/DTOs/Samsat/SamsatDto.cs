namespace Cassandra.Application.DTOs.Samsat;

public record SamsatDto(
    Guid Id,
    string Name,
    bool IsActive,
    IReadOnlyList<string> Cities);

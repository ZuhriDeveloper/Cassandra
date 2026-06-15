using System.Net.Http.Json;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Client for the SuperAdmin-only platform endpoints: dealer registry + user provisioning.
/// </summary>
public class PlatformApiClient(HttpClient http)
{
    // ── Dealers ─────────────────────────────────────────────────────────────────

    public Task<List<DealerDto>?> GetDealersAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<DealerDto>>("api/platform/dealers", ct);

    public async Task<(Guid? Id, List<string>? Errors)> RegisterDealerAsync(
        string name, string code, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/platform/dealers",
            new { name, code }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<RegisterDealerResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> RenameDealerAsync(
        Guid id, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(
            $"api/platform/dealers/{id}/name", new { name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetDealerStatusAsync(
        Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/platform/dealers/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Users ─────────────────────────────────────────────────────────────────

    public async Task<(string? UserId, List<string>? Errors)> RegisterUserAsync(
        string email, string fullName, string password, string role, Guid dealerId,
        CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/platform/users",
            new { email, fullName, password, role, dealerId }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<RegisterUserResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    internal static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage r)
    {
        try
        {
            var body = await r.Content.ReadFromJsonAsync<ErrorBody>();
            if (body?.Errors is { Count: > 0 })
                return body.Errors;
            if (body?.Message is not null)
                return [body.Message];
        }
        catch { }
        return [r.ReasonPhrase ?? "Unknown error"];
    }

    internal record ErrorBody(string? Message, List<string>? Errors);
}

public record DealerDto(Guid Id, string Name, string Code, bool IsActive);
public record RegisterDealerResponse(Guid Id);
public record RegisterUserResponse(string Id);

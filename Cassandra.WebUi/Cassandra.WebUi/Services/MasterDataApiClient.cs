using System.Net.Http.Json;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Client for dealer-scoped master data endpoints (Jabatan, Karyawan, etc.).
/// </summary>
public class MasterDataApiClient(HttpClient http)
{
    // ── Jabatan ───────────────────────────────────────────────────────────────

    public Task<List<JabatanDto>?> GetJabatanAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<JabatanDto>>("api/dealer/jabatan", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateJabatanAsync(
        string name, string description, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/jabatan",
            new { name, description }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<CreateJabatanResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateJabatanAsync(
        Guid id, string name, string description, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(
            $"api/dealer/jabatan/{id}", new { name, description }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetJabatanStatusAsync(
        Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/dealer/jabatan/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Karyawan ──────────────────────────────────────────────────────────────

    public Task<List<KaryawanDto>?> GetKaryawanAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<KaryawanDto>>("api/dealer/karyawan", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateKaryawanAsync(
        string name, string email, string ktpNumber, string gender,
        DateOnly hireDate, string phone, string? phoneAlt, string address,
        decimal salesLimit, Guid jabatanId, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/karyawan",
            new { name, email, ktpNumber, gender, hireDate, phone, phoneAlt, address, salesLimit, jabatanId }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<CreateKaryawanResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateKaryawanAsync(
        Guid id, string name, string email, string phone, string? phoneAlt,
        string address, Guid jabatanId, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(
            $"api/dealer/karyawan/{id}",
            new { name, email, phone, phoneAlt, address, jabatanId }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetKaryawanStatusAsync(
        Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/dealer/karyawan/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetKaryawanLimitAsync(
        Guid id, decimal salesLimit, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/dealer/karyawan/{id}/limit", new { salesLimit }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Kios ──────────────────────────────────────────────────────────────────

    public Task<List<KiosDto>?> GetKiosAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<KiosDto>>("api/dealer/kios", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateKiosAsync(
        string  code,
        string  name,
        string  phone,
        string? fax,
        string  address,
        Guid    picKaryawanId,
        decimal limit,
        CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/kios",
            new { code, name, phone, fax, address, picKaryawanId, limit }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<CreateKiosResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateKiosAsync(
        Guid    id,
        string  name,
        string  phone,
        string? fax,
        string  address,
        Guid    picKaryawanId,
        CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(
            $"api/dealer/kios/{id}",
            new { name, phone, fax, address, picKaryawanId }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetKiosStatusAsync(
        Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/dealer/kios/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage r)
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

    private record ErrorBody(string? Message, List<string>? Errors);
}

public record JabatanDto(Guid Id, string Name, string Description, bool IsActive);
public record CreateJabatanResponse(Guid Id);

public record KaryawanDto(
    Guid      Id,
    string    Name,
    string    Email,
    string    KtpNumber,
    string    Gender,
    DateOnly  HireDate,
    DateOnly? ResignDate,
    string    Phone,
    string?   PhoneAlt,
    string    Address,
    decimal   SalesLimit,
    Guid      JabatanId,
    bool      IsActive);
public record CreateKaryawanResponse(Guid Id);

public record KiosDto(
    Guid    Id,
    string  Code,
    string  Name,
    string  Phone,
    string? Fax,
    string  Address,
    Guid    PicKaryawanId,
    decimal Limit,
    bool    IsActive);
public record CreateKiosResponse(Guid Id);

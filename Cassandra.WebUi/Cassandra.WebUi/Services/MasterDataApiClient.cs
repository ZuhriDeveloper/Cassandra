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

    // ── Mediator ──────────────────────────────────────────────────────────────

    public Task<List<MediatorDto>?> GetMediatorsAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<MediatorDto>>("api/dealer/mediator", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateMediatorAsync(
        string  name,
        Guid    karyawanId,
        string  address,
        decimal limit,
        CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/mediator",
            new { name, karyawanId, address, limit }, ct);

        if (!response.IsSuccessStatusCode)
            return (null, await ReadErrorsAsync(response));

        var result = await response.Content
            .ReadFromJsonAsync<CreateMediatorResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateMediatorAsync(
        Guid    id,
        string  name,
        Guid    karyawanId,
        string  address,
        CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync(
            $"api/dealer/mediator/{id}",
            new { name, karyawanId, address }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetMediatorStatusAsync(
        Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync(
            $"api/dealer/mediator/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Warna ─────────────────────────────────────────────────────────────────

    public Task<List<WarnaDto>?> GetWarnaAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<WarnaDto>>("api/dealer/warna", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateWarnaAsync(string code, string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/warna", new { code, name }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateWarnaResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateWarnaAsync(Guid id, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/warna/{id}", new { name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetWarnaStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/warna/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── GrupTipeMotor ─────────────────────────────────────────────────────────

    public Task<List<GrupTipeMotorDto>?> GetGrupTipeMotorAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<GrupTipeMotorDto>>("api/dealer/grup-tipe-motor", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateGrupTipeMotorAsync(string code, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/grup-tipe-motor", new { code }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateGrupTipeMotorResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> SetGrupTipeMotorStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/grup-tipe-motor/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── TipeMotor ─────────────────────────────────────────────────────────────

    public Task<List<TipeMotorDto>?> GetTipeMotorAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<TipeMotorDto>>("api/dealer/tipe-motor", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateTipeMotorAsync(
        string code, Guid grupTipeMotorId, string shortName, string productCode,
        string wmsCode, string ahmCode, string engineNumberFormat, string chassisNumberFormat,
        decimal nettPrice, decimal orJakarta, decimal orTangerang, decimal bbnJakarta, decimal bbnTangerang,
        CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/tipe-motor",
            new { code, grupTipeMotorId, shortName, productCode, wmsCode, ahmCode, engineNumberFormat,
                  chassisNumberFormat, nettPrice, orJakarta, orTangerang, bbnJakarta, bbnTangerang }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateTipeMotorResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateTipeMotorAsync(
        Guid id, Guid grupTipeMotorId, string shortName, string productCode,
        string wmsCode, string ahmCode, string engineNumberFormat, string chassisNumberFormat,
        decimal nettPrice, decimal orJakarta, decimal orTangerang, decimal bbnJakarta, decimal bbnTangerang,
        CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/tipe-motor/{id}",
            new { grupTipeMotorId, shortName, productCode, wmsCode, ahmCode, engineNumberFormat,
                  chassisNumberFormat, nettPrice, orJakarta, orTangerang, bbnJakarta, bbnTangerang }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetTipeMotorStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/tipe-motor/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetTipeMotorColorsAsync(Guid id, List<Guid> warnaIds, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/tipe-motor/{id}/colors", new { warnaIds }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Kelengkapan ───────────────────────────────────────────────────────────

    public Task<List<KelengkapanDto>?> GetKelengkapanAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<KelengkapanDto>>("api/dealer/kelengkapan", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateKelengkapanAsync(string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/kelengkapan", new { name }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateKelengkapanResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateKelengkapanAsync(Guid id, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/kelengkapan/{id}", new { name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetKelengkapanStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/kelengkapan/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── MetodeKeuangan ────────────────────────────────────────────────────────────

    public Task<List<MetodeKeuanganDto>?> GetMetodeKeuanganAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<MetodeKeuanganDto>>("api/dealer/metode-keuangan", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateMetodeKeuanganAsync(string code, string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/metode-keuangan", new { code, name }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateMetodeKeuanganResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateMetodeKeuanganAsync(Guid id, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/metode-keuangan/{id}", new { name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetMetodeKeuanganStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/metode-keuangan/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── GlobalLeasing ─────────────────────────────────────────────────────────────

    public Task<List<GlobalLeasingDto>?> GetGlobalLeasingAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<GlobalLeasingDto>>("api/dealer/global-leasing", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateGlobalLeasingAsync(
        string code, string name, string phone, string? fax, string contact, string address, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/global-leasing",
            new { code, name, phone, fax, contact, address }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateGlobalLeasingResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateGlobalLeasingAsync(Guid id, string name, string phone, string? fax, string contact, string address, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/global-leasing/{id}",
            new { name, phone, fax, contact, address }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetGlobalLeasingStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/global-leasing/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── CabangLeasing ─────────────────────────────────────────────────────────────

    public Task<List<CabangLeasingDto>?> GetCabangLeasingAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<CabangLeasingDto>>("api/dealer/cabang-leasing", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateCabangLeasingAsync(
        string code, string name, string? phone, string? fax, string? contact, Guid globalLeasingId, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/cabang-leasing",
            new { code, name, phone, fax, contact, globalLeasingId }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateCabangLeasingResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateCabangLeasingAsync(Guid id, string name, string? phone, string? fax, string? contact, Guid globalLeasingId, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/cabang-leasing/{id}",
            new { name, phone, fax, contact, globalLeasingId }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetCabangLeasingStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/cabang-leasing/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── GrupTenor ─────────────────────────────────────────────────────────────────

    public Task<List<GrupTenorDto>?> GetGrupTenorAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<GrupTenorDto>>("api/dealer/grup-tenor", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateGrupTenorAsync(string code, string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/grup-tenor", new { code, name }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateGrupTenorResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateGrupTenorAsync(Guid id, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/grup-tenor/{id}", new { name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetGrupTenorStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/grup-tenor/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Tenor ─────────────────────────────────────────────────────────────────────

    public Task<List<TenorDto>?> GetTenorAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<TenorDto>>("api/dealer/tenor", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateTenorAsync(
        string code, string name, int months, Guid grupTenorId, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/tenor",
            new { code, name, months, grupTenorId }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateTenorResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateTenorAsync(Guid id, string name, int months, Guid grupTenorId, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/tenor/{id}",
            new { name, months, grupTenorId }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetTenorStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/tenor/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── DaftarHargaLeasing ────────────────────────────────────────────────────────

    public Task<List<DaftarHargaLeasingDto>?> GetDaftarHargaLeasingAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<DaftarHargaLeasingDto>>("api/dealer/daftar-harga-leasing", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateDaftarHargaLeasingAsync(
        Guid globalLeasingId, Guid grupTenorId, string name, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/daftar-harga-leasing",
            new { globalLeasingId, grupTenorId, name }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateDaftarHargaLeasingResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateDaftarHargaLeasingAsync(Guid id, Guid globalLeasingId, Guid grupTenorId, string name, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/daftar-harga-leasing/{id}",
            new { globalLeasingId, grupTenorId, name }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetDaftarHargaLeasingStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/daftar-harga-leasing/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetDaftarHargaLeasingItemsAsync(
        Guid id,
        List<(Guid GrupTipeMotorId, decimal Subsidi, decimal Incentive, decimal LainLain)> items,
        CancellationToken ct = default)
    {
        var payload = items.Select(x => new { grupTipeMotorId = x.GrupTipeMotorId, subsidi = x.Subsidi, incentive = x.Incentive, lainLain = x.LainLain }).ToList();
        var response = await http.PatchAsJsonAsync($"api/dealer/daftar-harga-leasing/{id}/items", new { items = payload }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── Discount ──────────────────────────────────────────────────────────────────

    public Task<List<DiscountDto>?> GetDiscountsAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<DiscountDto>>("api/dealer/discount", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateDiscountAsync(
        Guid daftarHargaLeasingId, string level, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/discount",
            new { daftarHargaLeasingId, level }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateDiscountResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> SetDiscountStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/discount/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetDiscountItemsAsync(
        Guid id, List<(Guid GrupTipeMotorId, decimal Amount)> items, CancellationToken ct = default)
    {
        var payload = items.Select(x => new { grupTipeMotorId = x.GrupTipeMotorId, amount = x.Amount }).ToList();
        var response = await http.PatchAsJsonAsync($"api/dealer/discount/{id}/items", new { items = payload }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── DiscountCash ──────────────────────────────────────────────────────────────

    public Task<List<DiscountCashDto>?> GetDiscountCashAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<DiscountCashDto>>("api/dealer/discount-cash", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateDiscountCashAsync(
        Guid tipeMotorId, decimal directDiscount, decimal channelDiscount, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/discount-cash",
            new { tipeMotorId, directDiscount, channelDiscount }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateDiscountCashResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateDiscountCashAsync(Guid id, decimal directDiscount, decimal channelDiscount, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/discount-cash/{id}",
            new { directDiscount, channelDiscount }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetDiscountCashStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/discount-cash/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── AlokasiDiskon ─────────────────────────────────────────────────────────────

    public Task<List<AlokasiDiskonDto>?> GetAlokasiDiskonAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<List<AlokasiDiskonDto>>("api/dealer/alokasi-diskon", ct);

    public async Task<(Guid? Id, List<string>? Errors)> CreateAlokasiDiskonAsync(
        Guid karyawanId, string discountLevel, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync("api/dealer/alokasi-diskon",
            new { karyawanId, discountLevel }, ct);
        if (!response.IsSuccessStatusCode) return (null, await ReadErrorsAsync(response));
        var result = await response.Content.ReadFromJsonAsync<CreateAlokasiDiskonResponse>(cancellationToken: ct);
        return (result?.Id, null);
    }

    public async Task<List<string>?> UpdateAlokasiDiskonAsync(Guid id, string discountLevel, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"api/dealer/alokasi-diskon/{id}", new { discountLevel }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    public async Task<List<string>?> SetAlokasiDiskonStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var response = await http.PatchAsJsonAsync($"api/dealer/alokasi-diskon/{id}/status", new { isActive }, ct);
        return response.IsSuccessStatusCode ? null : await ReadErrorsAsync(response);
    }

    // ── DF ────────────────────────────────────────────────────────────────────────

    public Task<DfDto?> GetDfAsync(CancellationToken ct = default)
        => http.GetFromJsonAsync<DfDto>("api/dealer/df", ct);

    public async Task<List<string>?> SetDfAsync(decimal discount, decimal interest, DateOnly startDate, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync("api/dealer/df", new { discount, interest, startDate }, ct);
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

public record MediatorDto(
    Guid    Id,
    string  Name,
    Guid    KaryawanId,
    string  Address,
    decimal Limit,
    bool    IsActive);
public record CreateMediatorResponse(Guid Id);

public record WarnaDto(Guid Id, string Code, string Name, bool IsActive);
public record CreateWarnaResponse(Guid Id);

public record GrupTipeMotorDto(Guid Id, string Code, bool IsActive);
public record CreateGrupTipeMotorResponse(Guid Id);

public record TipeMotorDto(
    Guid                Id,
    string              Code,
    Guid                GrupTipeMotorId,
    string              ShortName,
    string              ProductCode,
    string              WmsCode,
    string              AhmCode,
    string              EngineNumberFormat,
    string              ChassisNumberFormat,
    decimal             NettPrice,
    decimal             OrJakarta,
    decimal             OrTangerang,
    decimal             BbnJakarta,
    decimal             BbnTangerang,
    bool                IsActive,
    IReadOnlyList<Guid> WarnaIds);
public record CreateTipeMotorResponse(Guid Id);

public record KelengkapanDto(Guid Id, string Name, bool IsActive);
public record CreateKelengkapanResponse(Guid Id);

// Phase 3 — Financing & Leasing
public record MetodeKeuanganDto(Guid Id, string Code, string Name, bool IsActive);
public record CreateMetodeKeuanganResponse(Guid Id);

public record GlobalLeasingDto(Guid Id, string Code, string Name, string Phone, string? Fax, string Contact, string Address, bool IsActive);
public record CreateGlobalLeasingResponse(Guid Id);

public record CabangLeasingDto(Guid Id, string Code, string Name, string? Phone, string? Fax, string? Contact, Guid GlobalLeasingId, bool IsActive);
public record CreateCabangLeasingResponse(Guid Id);

public record GrupTenorDto(Guid Id, string Code, string Name, bool IsActive);
public record CreateGrupTenorResponse(Guid Id);

public record TenorDto(Guid Id, string Code, string Name, int Months, Guid GrupTenorId, bool IsActive);
public record CreateTenorResponse(Guid Id);

public record DaftarHargaLeasingItemDto(Guid GrupTipeMotorId, decimal Subsidi, decimal Incentive, decimal LainLain);
public record DaftarHargaLeasingDto(Guid Id, Guid GlobalLeasingId, Guid GrupTenorId, string Name, bool IsActive, IReadOnlyList<DaftarHargaLeasingItemDto> Items);
public record CreateDaftarHargaLeasingResponse(Guid Id);

public record DiscountItemDto(Guid GrupTipeMotorId, decimal Amount);
public record DiscountDto(Guid Id, Guid DaftarHargaLeasingId, string Level, bool IsActive, IReadOnlyList<DiscountItemDto> Items);
public record CreateDiscountResponse(Guid Id);

public record DiscountCashDto(Guid Id, Guid TipeMotorId, decimal DirectDiscount, decimal ChannelDiscount, bool IsActive);
public record CreateDiscountCashResponse(Guid Id);

public record AlokasiDiskonDto(Guid Id, Guid KaryawanId, string DiscountLevel, bool IsActive);
public record CreateAlokasiDiskonResponse(Guid Id);

public record DfDto(Guid Id, decimal Discount, decimal Interest, DateOnly StartDate, string UpdatedBy, DateTime UpdatedAt);

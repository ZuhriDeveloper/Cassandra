using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

/// <summary>
/// Sumber konten panduan in-app per halaman. Registry statis (singleton) yang memetakan
/// rute halaman ke <see cref="PageGuide"/>. Konten ditulis manual dan mencerminkan aturan
/// validasi (DataAnnotations) serta alur pada tiap halaman.
/// </summary>
public sealed partial class PageGuideProvider
{
    private readonly Dictionary<string, PageGuide> _guides;

    public PageGuideProvider()
    {
        var list = new List<PageGuide>();
        RegisterAccountAndPlatform(list);
        RegisterMasterData(list);
        RegisterKeuangan(list);
        RegisterBiroSamsat(list);
        RegisterInventori(list);
        RegisterPenjualan(list);

        _guides = list.ToDictionary(g => Normalize(g.Route), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Ambil panduan untuk sebuah rute, atau null bila belum tersedia.</summary>
    public PageGuide? Get(string? route)
        => string.IsNullOrWhiteSpace(route)
            ? null
            : _guides.TryGetValue(Normalize(route), out var g) ? g : null;

    /// <summary>Normalisasi rute: buang query string, hilangkan '/' berlebih, lowercase.</summary>
    private static string Normalize(string route)
    {
        var r = route.Trim();
        var q = r.IndexOf('?');
        if (q >= 0) r = r[..q];
        return "/" + r.Trim('/').ToLowerInvariant();
    }

    // ── Helper ringkas untuk authoring konten ───────────────────────────────────
    private static FieldGuide R(string label, string description, params string[] validations)
        => new(label, description, true, validations);

    private static FieldGuide O(string label, string description, params string[] validations)
        => new(label, description, false, validations);

    private static StatusInfo St(string status, string meaning) => new(status, meaning);

    // Nilai akses yang sering dipakai.
    private const string AccessMaster =
        "Admin: tambah, ubah, aktif/nonaktif • Sales & Kasir: lihat";
}

namespace Cassandra.WebUi.Components.Help;

/// <summary>
/// Penjelasan satu field/isian pada sebuah halaman, beserta aturan validasinya.
/// </summary>
/// <param name="Label">Label field seperti yang tampil pada form.</param>
/// <param name="Description">Penjelasan singkat fungsi field.</param>
/// <param name="Required">True bila field wajib diisi.</param>
/// <param name="Validations">Daftar aturan validasi singkat (mis. "Maksimal 100 karakter").</param>
/// <param name="Example">Contoh isian (opsional).</param>
public sealed record FieldGuide(
    string Label,
    string Description,
    bool Required,
    IReadOnlyList<string> Validations,
    string? Example = null);

/// <summary>Arti dari satu nilai status/alur pada halaman transaksi.</summary>
public sealed record StatusInfo(string Status, string Meaning);

/// <summary>
/// Panduan lengkap untuk satu halaman: tujuan, hak akses, langkah, field, dan status.
/// </summary>
public sealed record PageGuide
{
    /// <summary>Rute halaman (mis. "/dealer/jabatan").</summary>
    public required string Route { get; init; }

    /// <summary>Judul halaman yang ditampilkan di panel.</summary>
    public required string Title { get; init; }

    /// <summary>Ringkasan tujuan / fungsi halaman.</summary>
    public required string Purpose { get; init; }

    /// <summary>Siapa yang boleh melakukan apa (per peran).</summary>
    public string? Access { get; init; }

    /// <summary>Langkah penggunaan bernomor (opsional).</summary>
    public IReadOnlyList<string> Steps { get; init; } = [];

    /// <summary>Penjelasan tiap field beserta validasinya.</summary>
    public IReadOnlyList<FieldGuide> Fields { get; init; } = [];

    /// <summary>Legenda status/alur untuk halaman transaksi (opsional).</summary>
    public IReadOnlyList<StatusInfo> Statuses { get; init; } = [];

    /// <summary>Catatan tambahan (opsional).</summary>
    public string? Notes { get; init; }
}

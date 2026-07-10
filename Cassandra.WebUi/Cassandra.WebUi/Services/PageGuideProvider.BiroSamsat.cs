using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private static void RegisterBiroSamsat(List<PageGuide> l)
    {
        // ── Samsat ───────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/samsat",
            Title = "Samsat",
            Purpose = "Mengelola daftar kantor Samsat untuk pengurusan STNK/pajak kendaraan.",
            Access = AccessMaster,
            Fields =
            [
                R("Nama", "Nama kantor Samsat.", "Wajib diisi", "Maksimal 200 karakter"),
            ],
        });

        // ── Biro ─────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/biro",
            Title = "Biro",
            Purpose = "Mengelola biro jasa pengurusan dokumen (STNK/BPKB) beserta tarif PPH.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode biro.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama biro jasa.", "Wajib diisi", "Maksimal 200 karakter"),
                O("Telp", "Nomor telepon."),
                O("Fax", "Nomor faks."),
                O("PIC", "Penanggung jawab."),
                O("Alamat", "Alamat biro."),
                O("Tarif PPH %", "Persentase pajak penghasilan atas jasa."),
            ],
        });

        // ── Biaya Biro Jasa ──────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/biaya-biro-jasa",
            Title = "Biaya Biro Jasa",
            Purpose = "Menetapkan tarif jasa biro per kombinasi Samsat + Biro, dirinci per tipe "
                    + "motor.",
            Access = AccessMaster,
            Steps =
            [
                "Pilih Samsat dan Biro.",
                "Isi rincian biaya per Tipe Motor: Biaya STNK (Rp) dan Notice (Rp).",
                "Simpan.",
            ],
            Fields =
            [
                R("Samsat", "Dipilih dari master Samsat.", "Wajib dipilih"),
                R("Biro", "Dipilih dari master Biro.", "Wajib dipilih"),
                R("Rincian: Tipe Motor + Biaya STNK + Notice", "Tarif per tipe motor.",
                  "Wajib diisi"),
            ],
        });

        // ── Jenis Pengeluaran ────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/expense-type",
            Title = "Jenis Pengeluaran",
            Purpose = "Mengelola kategori jenis pengeluaran/biaya.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode Jenis Pengeluaran", "Kode kategori.", "Wajib diisi",
                  "Maksimal 20 karakter", "Harus unik"),
                R("Nama Jenis Pengeluaran", "Nama kategori pengeluaran.", "Wajib diisi",
                  "Maksimal 200 karakter"),
            ],
        });

        // ── Ledger ───────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/ledger",
            Title = "Ledger (Akun)",
            Purpose = "Mengelola daftar akun buku besar (ledger).",
            Access = AccessMaster,
            Fields =
            [
                R("Nama Akun", "Nama akun buku besar.", "Wajib diisi", "Maksimal 200 karakter"),
            ],
        });

        // ── Pelanggaran Wilayah ──────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/pelanggaran-wilayah",
            Title = "Pelanggaran Wilayah",
            Purpose = "Menetapkan biaya tambahan bila penjualan keluar dari wilayah yang "
                    + "ditentukan.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode Area", "Kode area/wilayah.", "Wajib diisi", "Maksimal 50 karakter",
                  "Harus unik"),
                R("Biaya Tambahan (Rp)", "Biaya tambahan untuk area tersebut.", "Wajib diisi",
                  "Tidak boleh negatif"),
            ],
        });
    }
}

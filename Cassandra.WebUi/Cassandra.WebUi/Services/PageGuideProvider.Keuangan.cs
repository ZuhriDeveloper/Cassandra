using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private static void RegisterKeuangan(List<PageGuide> l)
    {
        // ── Metode Keuangan ──────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/metode-keuangan",
            Title = "Metode Keuangan",
            Purpose = "Mengelola metode/cara pembayaran atau sumber dana.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode metode.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama metode keuangan.", "Wajib diisi", "Maksimal 100 karakter"),
            ],
        });

        // ── Global Leasing ───────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/global-leasing",
            Title = "Global Leasing",
            Purpose = "Mengelola perusahaan leasing pada tingkat pusat.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode leasing.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama perusahaan leasing.", "Wajib diisi", "Maksimal 200 karakter"),
                R("Telepon", "Nomor telepon.", "Wajib diisi", "Maksimal 30 karakter"),
                O("Fax", "Nomor faks.", "Maksimal 30 karakter"),
                R("Contact", "Nama kontak/PIC.", "Wajib diisi", "Maksimal 100 karakter"),
                R("Alamat", "Alamat kantor pusat.", "Wajib diisi", "Maksimal 300 karakter"),
            ],
        });

        // ── Cabang Leasing ───────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/cabang-leasing",
            Title = "Cabang Leasing",
            Purpose = "Mengelola cabang dari perusahaan leasing dan mengaitkannya ke Global "
                    + "Leasing.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode cabang.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama cabang leasing.", "Wajib diisi", "Maksimal 200 karakter"),
                R("Global Leasing", "Induk leasing, dipilih dari master Global Leasing.",
                  "Wajib dipilih"),
                O("Telepon", "Nomor telepon cabang.", "Maksimal 30 karakter"),
                O("Fax", "Nomor faks.", "Maksimal 30 karakter"),
                O("Contact", "Nama kontak cabang.", "Maksimal 100 karakter"),
            ],
            Notes = "Buat Global Leasing terlebih dahulu agar dapat dipilih sebagai induk cabang.",
        });

        // ── Grup Tenor ───────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/grup-tenor",
            Title = "Grup Tenor",
            Purpose = "Mengelola pengelompokan pilihan tenor (jangka waktu cicilan).",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode grup tenor.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama grup tenor.", "Wajib diisi", "Maksimal 100 karakter"),
            ],
        });

        // ── Tenor ────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/tenor",
            Title = "Tenor",
            Purpose = "Mengelola pilihan tenor cicilan (dalam bulan) dalam suatu grup tenor.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode tenor.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama tenor.", "Wajib diisi", "Maksimal 100 karakter"),
                R("Bulan", "Jumlah bulan cicilan.", "Wajib diisi", "Antara 1 dan 360")
                    with { Example = "12, 24, 36" },
                R("Grup Tenor", "Grup induk, dipilih dari master Grup Tenor.", "Wajib dipilih"),
            ],
        });

        // ── Daftar Harga Leasing ─────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/daftar-harga-leasing",
            Title = "Daftar Harga Leasing",
            Purpose = "Menyusun daftar harga/skema kredit per leasing dan grup tenor, dengan "
                    + "rincian subsidi dan insentif per grup tipe motor.",
            Access = AccessMaster,
            Steps =
            [
                "Isi header daftar: Nama Daftar, Global Leasing, dan Grup Tenor.",
                "Tambahkan baris rincian per Grup Tipe Motor (Subsidi, Incentive, Lain-lain); "
                + "kolom Total dihitung otomatis.",
                "Simpan daftar.",
            ],
            Fields =
            [
                R("Nama Daftar", "Nama daftar harga leasing.", "Wajib diisi",
                  "Maksimal 200 karakter"),
                R("Global Leasing", "Leasing terkait.", "Wajib dipilih"),
                R("Grup Tenor", "Grup tenor yang berlaku.", "Wajib dipilih"),
                O("Rincian: Subsidi / Incentive / Lain-lain", "Komponen nilai per grup tipe "
                  + "motor; Total dihitung otomatis."),
            ],
        });

        // ── Diskon Kredit ────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/discount",
            Title = "Diskon Kredit",
            Purpose = "Mengatur diskon untuk penjualan kredit, ditentukan per level dan per grup "
                    + "tipe motor berdasarkan Daftar Harga Leasing.",
            Access = AccessMaster,
            Fields =
            [
                R("Daftar Harga Leasing", "Dipilih dari master Daftar Harga Leasing.",
                  "Wajib dipilih"),
                R("Level", "Level/tingkat diskon.", "Wajib diisi", "Maksimal 10 karakter"),
                R("Rincian: Grup Tipe Motor + Amount", "Nilai diskon per grup tipe motor.",
                  "Wajib diisi"),
            ],
        });

        // ── Diskon Cash ──────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/discount-cash",
            Title = "Diskon Cash",
            Purpose = "Mengatur diskon untuk penjualan tunai per tipe motor (jalur direct dan "
                    + "channel).",
            Access = AccessMaster,
            Fields =
            [
                R("Tipe Motor", "Dipilih dari master Tipe Motor.", "Wajib dipilih"),
                R("Diskon Direct (Rp)", "Diskon jalur langsung.", "Wajib diisi",
                  "Tidak boleh negatif"),
                R("Diskon Channel (Rp)", "Diskon jalur channel/mitra.", "Wajib diisi",
                  "Tidak boleh negatif"),
            ],
        });

        // ── Alokasi Diskon ───────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/alokasi-diskon",
            Title = "Alokasi Diskon",
            Purpose = "Menetapkan level diskon maksimum yang boleh diberikan oleh tiap karyawan.",
            Access = AccessMaster,
            Fields =
            [
                R("Karyawan", "Dipilih dari master Karyawan.", "Wajib dipilih"),
                R("Level Diskon", "Level diskon untuk karyawan tersebut.", "Wajib diisi",
                  "Maksimal 10 karakter"),
            ],
        });

        // ── DF (Biaya Pembiayaan) ────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/df",
            Title = "DF (Biaya Pembiayaan)",
            Purpose = "Mengatur satu konfigurasi DF (Dealer Financing) yang berlaku: persentase "
                    + "diskon, persentase bunga, dan tanggal mulai berlaku. Halaman ini bukan "
                    + "daftar, melainkan satu konfigurasi tunggal.",
            Access = "Admin: menyimpan konfigurasi • Sales & Kasir: lihat",
            Steps =
            [
                "Kartu 'Konfigurasi DF Saat Ini' menampilkan nilai yang sedang berlaku.",
                "Isi Diskon DF (%), Bunga DF (%), dan Tanggal Berlaku pada formulir.",
                "Klik Simpan Konfigurasi DF (hanya Admin).",
            ],
            Fields =
            [
                R("Diskon DF (%)", "Persentase diskon pembiayaan.", "Wajib diisi",
                  "Antara 0 dan 100"),
                R("Bunga DF (%)", "Persentase bunga pembiayaan.", "Wajib diisi",
                  "Antara 0 dan 100"),
                R("Tanggal Berlaku", "Tanggal mulai konfigurasi berlaku.", "Wajib diisi"),
            ],
            Notes = "Menyimpan nilai baru akan menggantikan konfigurasi yang berlaku sebelumnya.",
        });
    }
}

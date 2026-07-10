using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private static void RegisterMasterData(List<PageGuide> l)
    {
        // ── Jabatan ──────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/jabatan",
            Title = "Jabatan",
            Purpose = "Mengelola daftar jabatan karyawan (mis. Kepala Mekanik, Sales Counter).",
            Access = AccessMaster,
            Fields =
            [
                R("Nama Jabatan", "Nama jabatan.", "Wajib diisi", "Maksimal 100 karakter")
                    with { Example = "Kepala Mekanik" },
                O("Deskripsi", "Keterangan singkat tentang jabatan.", "Maksimal 500 karakter"),
            ],
        });

        // ── Karyawan ─────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/karyawan",
            Title = "Karyawan",
            Purpose = "Mengelola data karyawan dealer beserta jabatan dan batas transaksinya.",
            Access = AccessMaster,
            Fields =
            [
                R("Nama Lengkap", "Nama karyawan.", "Wajib diisi", "Maksimal 200 karakter"),
                R("Email", "Email karyawan.", "Wajib diisi", "Format email valid",
                  "Maksimal 200 karakter"),
                R("No. KTP", "Nomor Kartu Tanda Penduduk.", "Wajib diisi", "Maksimal 50 karakter"),
                R("Jenis Kelamin", "Laki-laki atau Perempuan.", "Wajib dipilih"),
                R("Tgl. Masuk", "Tanggal mulai bekerja.", "Wajib diisi"),
                R("Jabatan", "Jabatan karyawan, diambil dari master Jabatan.", "Wajib dipilih"),
                R("No. Telepon", "Nomor telepon utama.", "Wajib diisi", "Maksimal 20 karakter"),
                O("No. Telepon 2", "Nomor telepon alternatif.", "Maksimal 20 karakter"),
                O("Alamat", "Alamat karyawan.", "Maksimal 500 karakter"),
                O("Sales Limit (Rp)", "Batas nilai transaksi yang boleh ditangani karyawan.",
                  "Tidak boleh negatif"),
            ],
            Notes = "Pastikan data Jabatan sudah dibuat lebih dulu agar dapat dipilih di sini.",
        });

        // ── Kios ─────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/kios",
            Title = "Kios",
            Purpose = "Mengelola kios/showroom tempat unit disimpan dan dijual.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode kios.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama Kios", "Nama kios/showroom.", "Wajib diisi", "Maksimal 200 karakter"),
                R("PIC (Karyawan)", "Penanggung jawab kios, dipilih dari master Karyawan.",
                  "Wajib dipilih"),
                R("No. Telepon", "Nomor telepon kios.", "Wajib diisi", "Maksimal 20 karakter"),
                O("No. Fax", "Nomor faks.", "Maksimal 20 karakter"),
                O("Alamat", "Alamat kios.", "Maksimal 500 karakter"),
                O("Limit (Rp)", "Batas nilai/plafon kios.", "Tidak boleh negatif"),
            ],
        });

        // ── Mediator ─────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/mediator",
            Title = "Mediator",
            Purpose = "Mengelola mediator (perantara penjualan) yang dikaitkan dengan seorang "
                    + "sales.",
            Access = AccessMaster,
            Fields =
            [
                R("Nama Mediator", "Nama mediator/perantara.", "Wajib diisi",
                  "Maksimal 200 karakter"),
                R("Karyawan (Sales)", "Sales pendamping, dipilih dari master Karyawan.",
                  "Wajib dipilih"),
                O("Alamat", "Alamat mediator.", "Maksimal 500 karakter"),
                O("Limit (Rp)", "Batas nilai untuk mediator.", "Tidak boleh negatif"),
            ],
        });

        // ── Warna ────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/warna",
            Title = "Warna",
            Purpose = "Mengelola daftar warna unit motor.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode warna.", "Wajib diisi", "Maksimal 20 karakter", "Harus unik"),
                R("Nama", "Nama warna.", "Wajib diisi", "Maksimal 100 karakter"),
            ],
        });

        // ── Grup Tipe Motor ──────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/grup-tipe-motor",
            Title = "Grup Tipe Motor",
            Purpose = "Mengelola pengelompokan tipe motor (mis. matic, bebek, sport) yang "
                    + "dipakai pada skema harga dan diskon.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode/nama grup tipe motor.", "Wajib diisi", "Maksimal 50 karakter",
                  "Harus unik"),
            ],
        });

        // ── Tipe Motor ───────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/tipe-motor",
            Title = "Tipe Motor",
            Purpose = "Mengelola tipe/varian motor beserta kode, harga, dan komponen biaya per "
                    + "wilayah.",
            Access = AccessMaster,
            Fields =
            [
                R("Kode", "Kode tipe motor.", "Wajib diisi", "Maksimal 50 karakter", "Harus unik"),
                R("Grup Tipe Motor", "Grup induk, dipilih dari master Grup Tipe Motor.",
                  "Wajib dipilih"),
                R("Nama Singkat", "Nama pendek tipe motor.", "Wajib diisi",
                  "Maksimal 100 karakter"),
                R("Kode Produk", "Kode produk.", "Wajib diisi", "Maksimal 50 karakter"),
                R("Kode WMS", "Kode gudang/WMS.", "Wajib diisi", "Maksimal 50 karakter"),
                R("Kode AHM", "Kode dari prinsipal (AHM).", "Wajib diisi", "Maksimal 50 karakter"),
                R("Format Nomor Mesin", "Pola untuk validasi nomor mesin unit.", "Wajib diisi",
                  "Maksimal 100 karakter"),
                R("Format Nomor Rangka", "Pola untuk validasi nomor rangka unit.", "Wajib diisi",
                  "Maksimal 100 karakter"),
                R("Harga Nett (Rp)", "Harga dasar unit.", "Wajib diisi", "Tidak boleh negatif"),
                O("OR Jakarta", "Biaya Off Road wilayah Jakarta.", "Tidak boleh negatif"),
                O("OR Tangerang", "Biaya Off Road wilayah Tangerang.", "Tidak boleh negatif"),
                O("BBN Jakarta", "Bea Balik Nama wilayah Jakarta.", "Tidak boleh negatif"),
                O("BBN Tangerang", "Bea Balik Nama wilayah Tangerang.", "Tidak boleh negatif"),
            ],
            Notes = "OR = Off Road, BBN = Bea Balik Nama; nilainya berbeda antar wilayah.",
        });

        // ── Kelengkapan ──────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/kelengkapan",
            Title = "Kelengkapan",
            Purpose = "Mengelola daftar item kelengkapan unit (mis. helm, buku servis, toolkit) "
                    + "yang dicatat saat penerimaan dan mutasi unit.",
            Access = AccessMaster,
            Fields =
            [
                R("Nama", "Nama item kelengkapan.", "Wajib diisi", "Maksimal 200 karakter"),
            ],
        });
    }
}

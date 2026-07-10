using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private static void RegisterPenjualan(List<PageGuide> l)
    {
        // ── Registrasi Penjualan ─────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/registrasi-penjualan",
            Title = "Registrasi Penjualan",
            Purpose = "Mencatat transaksi penjualan motor ke konsumen (tunai/kredit): data unit, "
                    + "konsumen, harga, dan persetujuan diskon.",
            Access = "Admin, Sales (membuat & approve)",
            Steps =
            [
                "Klik Tambah Penjualan untuk membuka formulir.",
                "Isi data penjualan, unit, harga, dan data konsumen.",
                "Klik Simpan. Transaksi baru berstatus PENDING.",
                "Untuk menyetujui, klik Approve pada baris, isi Diskon yang Disetujui, lalu "
                + "simpan → status APPROVED.",
                "Klik baris untuk melihat detail lengkap.",
            ],
            Fields =
            [
                R("No. Penjualan", "Nomor transaksi penjualan.", "Wajib diisi"),
                R("Tanggal Penjualan", "Tanggal transaksi.", "Wajib diisi"),
                R("Sales / Karyawan", "Sales penanggung jawab.", "Wajib dipilih"),
                R("Kios", "Kios asal unit.", "Wajib dipilih"),
                O("Mediator", "Perantara penjualan (bila ada)."),
                R("No. Mesin", "Nomor mesin unit yang dijual.", "Wajib diisi"),
                R("No. Rangka", "Nomor rangka unit yang dijual.", "Wajib diisi"),
                R("Metode Penjualan", "CASH atau CREDIT.", "Wajib dipilih"),
                R("Tipe Penjualan", "Kategori penjualan.", "Wajib dipilih"),
                R("No. Serah Terima Kendaraan", "Nomor dokumen serah terima kendaraan (STK).",
                  "Wajib diisi"),
                O("No. Tanda Terima Sementara", "Nomor TTS (bila ada)."),
                R("Harga Total", "Total harga jual.", "Wajib diisi"),
                O("Off Road / BBN", "Komponen biaya Off Road dan Bea Balik Nama."),
                O("Diskon yang Diminta", "Diskon yang diajukan (disetujui saat Approve)."),
                O("DP / Angsuran / Daftar Harga Leasing / Tenor", "Khusus penjualan CREDIT."),
                R("Nama Konsumen", "Nama pembeli.", "Wajib diisi"),
                R("Alamat", "Alamat konsumen.", "Wajib diisi"),
                R("Telepon", "Nomor telepon konsumen.", "Wajib diisi"),
                R("Diskon yang Disetujui", "Diisi pada saat Approve.", "Wajib diisi saat Approve"),
            ],
            Statuses =
            [
                St("PENDING", "Menunggu persetujuan (approve)."),
                St("APPROVED", "Sudah disetujui, diskon final ditetapkan."),
                St("SENT", "Unit sudah dikirim ke konsumen."),
                St("VOID", "Transaksi dibatalkan."),
            ],
            Notes = "Memilih Metode CREDIT menampilkan isian pembiayaan (DP, angsuran, leasing, "
                  + "tenor). Diskon final ditentukan pada langkah Approve.",
        });

        // ── Pengiriman Motor ─────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/pengiriman-motor",
            Title = "Pengiriman Motor",
            Purpose = "Mencatat pengiriman unit ke konsumen atas suatu registrasi penjualan.",
            Access = "Admin, Sales",
            Steps =
            [
                "Klik Buat Pengiriman untuk membuka formulir.",
                "Pilih Registrasi Penjualan terkait.",
                "Isi Driver 1, Tanggal Kirim, serta Driver 2/Zona bila ada.",
                "Klik Simpan.",
            ],
            Fields =
            [
                R("Registrasi Penjualan", "Transaksi penjualan yang dikirim.", "Wajib dipilih"),
                O("No. Mesin", "Nomor mesin unit (mengikuti penjualan)."),
                R("Driver 1", "Pengemudi/pengantar utama.", "Wajib diisi"),
                O("Driver 2", "Pengemudi pendamping."),
                R("Tanggal Kirim", "Tanggal pengiriman.", "Wajib diisi"),
                O("Zona", "Zona/area pengiriman."),
            ],
            Notes = "Setelah pengiriman dicatat, status penjualan menjadi SENT dan status unit "
                  + "menjadi TERKIRIM.",
        });

        // ── STNK ─────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/stnk",
            Title = "STNK",
            Purpose = "Mengelola proses penerbitan STNK untuk tiap penjualan, dari faktur hingga "
                    + "serah terima ke konsumen. Prosesnya bertahap mengikuti status.",
            Access = "Admin, Sales",
            Steps =
            [
                "Klik Buat STNK, pilih Registrasi Penjualan, isi Tanggal/Nama/Alamat Faktur → "
                + "status RECEIVE FAKTUR.",
                "Klik Proses: isi Tanggal Proses, Biro, No. Invoice → status PROCESS.",
                "Klik Terima: isi Tanggal Terima, No. Polisi, Biro, No. STNK, No. Invoice, "
                + "Wilayah, dan rincian biaya → status RECEIVE.",
                "Klik Serah Terima: isi Tanggal Serah Terima dan Penerima → status HANDOVER.",
            ],
            Fields =
            [
                R("Registrasi Penjualan", "Transaksi penjualan terkait.", "Wajib dipilih"),
                R("Tanggal Faktur", "Tanggal faktur.", "Wajib diisi"),
                R("Nama Faktur", "Nama pada faktur.", "Wajib diisi"),
                R("Alamat Faktur", "Alamat pada faktur.", "Wajib diisi"),
                R("Tanggal Proses / Biro / No. Invoice", "Diisi pada tahap Proses.",
                  "Wajib saat tahap Proses"),
                R("No. Polisi / No. STNK / Wilayah", "Diisi pada tahap Terima.",
                  "Wajib saat tahap Terima"),
                O("Rincian Biaya (STNK, Notice, Progresif, BBN, PNBP, Admin, Lain, Jasa, PPH)",
                  "Komponen biaya, dicatat pada tahap Terima."),
                R("Tanggal Serah Terima / Penerima", "Diisi pada tahap Serah Terima.",
                  "Wajib saat tahap Serah Terima"),
            ],
            Statuses =
            [
                St("RECEIVE FAKTUR", "Faktur diterima (data awal dibuat)."),
                St("PROCESS", "Sedang diproses melalui biro."),
                St("RECEIVE", "STNK diterima dari Samsat/biro."),
                St("HANDOVER", "STNK diserahkan ke konsumen."),
            ],
        });

        // ── BPKB ─────────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/bpkb",
            Title = "BPKB",
            Purpose = "Mengelola proses BPKB dari permintaan hingga serah terima ke konsumen.",
            Access = "Admin, Sales, Kasir (sesuai kewenangan)",
            Steps =
            [
                "Data permintaan BPKB muncul berstatus REQUEST.",
                "Klik Terima BPKB: isi Tanggal Terima, No. BPKB, No. Buku → status RECEIVE.",
                "Klik Serah Terima: isi Tanggal Serah Terima dan Penerima → status HANDOVER.",
            ],
            Fields =
            [
                R("Tanggal Terima / No. BPKB / No. Buku", "Diisi pada tahap Terima BPKB.",
                  "Wajib saat tahap Terima"),
                R("Tanggal Serah Terima / Penerima", "Diisi pada tahap Serah Terima.",
                  "Wajib saat tahap Serah Terima"),
            ],
            Statuses =
            [
                St("REQUEST", "Permintaan BPKB tercatat."),
                St("RECEIVE", "BPKB diterima dari Samsat."),
                St("HANDOVER", "BPKB diserahkan ke konsumen."),
            ],
        });
    }
}

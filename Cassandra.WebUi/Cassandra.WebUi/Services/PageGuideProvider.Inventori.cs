using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private const string AccessInventori = "Admin, Sales, Kasir (sesuai kewenangan)";

    private static void RegisterInventori(List<PageGuide> l)
    {
        // ── Surat Order (SO) ─────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/so",
            Title = "Surat Order (SO)",
            Purpose = "Membuat surat pemesanan (order) unit motor ke prinsipal — dasar untuk "
                    + "penerimaan stok.",
            Access = AccessInventori,
            Steps =
            [
                "Klik Tambah SO untuk membuka formulir.",
                "Isi header SO dan tambahkan baris item (Tipe Motor, Warna, Qty, Harga Nett); "
                + "subtotal dihitung otomatis.",
                "Klik Simpan. Gunakan Tutup Form untuk menutup formulir.",
                "Pada daftar, klik baris untuk melihat rincian; klik Selesaikan untuk menandai "
                + "SO menjadi SELESAI.",
            ],
            Fields =
            [
                R("No. SO", "Nomor surat order.", "Wajib diisi"),
                R("Tanggal SO", "Tanggal order.", "Wajib diisi"),
                R("Jatuh Tempo", "Tanggal jatuh tempo.", "Wajib diisi"),
                R("Jenis Pembayaran", "Tunai/kredit dsb.", "Wajib dipilih"),
                R("Metode Keuangan", "Dipilih dari master Metode Keuangan.", "Wajib dipilih"),
                O("Subsidi", "Nilai subsidi."),
                O("Diskon Cash %", "Persentase diskon tunai.", "Antara 0 dan 100"),
                O("DF", "Komponen pembiayaan (DF)."),
                R("Baris item (Tipe Motor, Warna, Qty, Harga Nett)", "Unit yang dipesan.",
                  "Minimal 1 baris", "Qty lebih dari 0"),
            ],
            Statuses =
            [
                St("AKTIF", "SO masih berjalan / belum selesai."),
                St("SELESAI", "SO telah diselesaikan (unit diterima)."),
                St("RETUR", "SO memiliki retur."),
            ],
        });

        // ── Stok Unit ────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/stock",
            Title = "Stok Unit",
            Purpose = "Menampilkan seluruh unit motor fisik beserta status dan lokasinya. "
                    + "Halaman ini untuk memantau stok; data terisi otomatis dari proses "
                    + "Penerimaan SO.",
            Access = "Admin, Sales, Kasir",
            Notes = "Halaman ini bersifat lihat saja (read-only). Perhatikan kolom Status untuk "
                  + "mengetahui ketersediaan tiap unit.",
            Statuses =
            [
                St("TERSEDIA", "Unit siap dijual."),
                St("DIPESAN", "Unit sudah dipesan konsumen."),
                St("TERJUAL", "Unit sudah terjual."),
                St("TERKIRIM", "Unit sudah dikirim ke konsumen."),
                St("RETUR", "Unit dikembalikan/retur."),
            ],
        });

        // ── Penerimaan SO ────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/so-penerimaan",
            Title = "Penerimaan SO",
            Purpose = "Mencatat penerimaan unit (goods receipt) atas suatu SO. Setiap unit yang "
                    + "diterima masuk menjadi Stok Unit.",
            Access = AccessInventori,
            Steps =
            [
                "Klik Tambah Penerimaan untuk membuka formulir.",
                "Isi No. Surat Jalan, Tanggal Surat Jalan, dan Nomor SO.",
                "Untuk tiap unit, isi Tipe Motor, Warna, No. Mesin, No. Rangka, Kios, dan Tahun "
                + "Rakit; catat Kelengkapan (Qty & Catatan) bila ada.",
                "Klik Simpan. Unit yang diterima otomatis menambah Stok Unit.",
            ],
            Fields =
            [
                R("No. Surat Jalan", "Nomor surat jalan pengirim.", "Wajib diisi"),
                R("Tanggal Surat Jalan", "Tanggal surat jalan.", "Wajib diisi"),
                R("Nomor SO", "SO yang diterima.", "Wajib dipilih"),
                R("Unit (Tipe Motor, Warna, No. Mesin, No. Rangka, Kios, Tahun Rakit)",
                  "Data tiap unit yang diterima.", "Wajib diisi",
                  "No. Mesin & No. Rangka harus unik"),
                O("Kelengkapan (Qty, Catatan)", "Item kelengkapan yang menyertai unit."),
            ],
        });

        // ── Retur SO ─────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/so-retur",
            Title = "Retur SO",
            Purpose = "Mencatat pengembalian (retur) unit atas suatu SO.",
            Access = AccessInventori,
            Steps =
            [
                "Klik Tambah Retur untuk membuka formulir.",
                "Isi No. Retur, pilih SO, isi Tanggal Retur dan Alasan.",
                "Tambahkan baris item retur (Tipe Motor, Warna, Qty, Harga Nett).",
                "Klik Simpan.",
            ],
            Fields =
            [
                R("No. Retur", "Nomor retur.", "Wajib diisi"),
                R("SO", "SO yang diretur.", "Wajib dipilih"),
                R("Tanggal Retur", "Tanggal retur.", "Wajib diisi"),
                R("Alasan", "Alasan pengembalian.", "Wajib diisi"),
                R("Baris item (Tipe Motor, Warna, Qty, Harga Nett)", "Unit yang diretur.",
                  "Minimal 1 baris"),
            ],
        });

        // ── Mutasi Stok ──────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/mutasi",
            Title = "Mutasi Stok",
            Purpose = "Memindahkan unit stok antar kios.",
            Access = AccessInventori,
            Steps =
            [
                "Klik Tambah Mutasi untuk membuka formulir.",
                "Isi No. Mutasi, Tanggal Mutasi, pilih Kios Asal dan Kios Tujuan.",
                "Pilih (centang) unit yang akan dipindahkan beserta kelengkapannya.",
                "Klik Simpan.",
            ],
            Fields =
            [
                R("No. Mutasi", "Nomor mutasi.", "Wajib diisi"),
                R("Tanggal Mutasi", "Tanggal pemindahan.", "Wajib diisi"),
                R("Kios Asal", "Kios sumber unit.", "Wajib dipilih"),
                R("Kios Tujuan", "Kios tujuan unit.", "Wajib dipilih",
                  "Harus berbeda dari Kios Asal"),
                R("Pilihan Unit + Kelengkapan (Qty)", "Unit yang dipindah beserta kelengkapan.",
                  "Minimal 1 unit dipilih"),
            ],
        });
    }
}

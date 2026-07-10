using Cassandra.WebUi.Components.Help;

namespace Cassandra.WebUi.Services;

public sealed partial class PageGuideProvider
{
    private static void RegisterAccountAndPlatform(List<PageGuide> l)
    {
        // ── Beranda ──────────────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/",
            Title = "Beranda",
            Purpose = "Halaman sambutan setelah masuk. Menampilkan nama pengguna yang sedang "
                    + "login dan menjadi titik awal untuk berpindah ke menu lain melalui panel "
                    + "samping kiri.",
            Access = "Semua pengguna yang sudah masuk",
        });

        // ── Ubah Kata Sandi ──────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/account/change-password",
            Title = "Ubah Kata Sandi",
            Purpose = "Mengganti kata sandi akun Anda saat sedang login.",
            Access = "Semua pengguna yang sudah masuk",
            Steps =
            [
                "Isi Kata Sandi Saat Ini.",
                "Isi Kata Sandi Baru dan ulangi pada kolom konfirmasi.",
                "Klik Simpan.",
            ],
            Fields =
            [
                R("Kata Sandi Saat Ini", "Kata sandi yang sedang digunakan sekarang.",
                  "Wajib diisi"),
                R("Kata Sandi Baru", "Kata sandi pengganti.",
                  "Wajib diisi", "Minimal 8 karakter", "Minimal 1 angka",
                  "Minimal 1 huruf kapital", "Harus berbeda dari kata sandi saat ini"),
                R("Ulangi Kata Sandi Baru", "Konfirmasi kata sandi baru.",
                  "Wajib diisi", "Harus sama dengan Kata Sandi Baru"),
            ],
        });

        // ── Platform ▸ Dealer ────────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/platform/dealers",
            Title = "Dealer (Platform)",
            Purpose = "Mengelola daftar dealer (cabang usaha): mendaftarkan dealer baru, "
                    + "mengubah nama, serta mengaktifkan/menonaktifkan dealer.",
            Access = "SuperAdmin",
            Steps =
            [
                "Isi Nama Dealer dan Kode Dealer, lalu klik Daftarkan Dealer.",
                "Untuk mengubah nama, klik ikon pensil pada baris dealer, ubah nama, lalu Simpan.",
                "Gunakan tombol alih status untuk menonaktifkan/mengaktifkan dealer.",
            ],
            Fields =
            [
                R("Nama Dealer", "Nama dealer.",
                  "Wajib diisi", "Maksimal 200 karakter"),
                R("Kode Dealer", "Kode unik dealer yang bersifat permanen.",
                  "Wajib diisi", "Maksimal 30 karakter",
                  "Hanya huruf kapital, angka, dan tanda hubung",
                  "Tidak dapat diubah setelah dibuat", "Harus unik")
                    with { Example = "BDG-01" },
            ],
            Notes = "Kode dealer tidak bisa diganti. Bila kode salah, nonaktifkan dealer lalu "
                  + "buat yang baru.",
        });

        // ── Platform ▸ Pengguna ──────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/platform/users",
            Title = "Pengguna (Platform)",
            Purpose = "Membuat akun pengguna untuk sebuah dealer beserta perannya.",
            Access = "SuperAdmin",
            Steps =
            [
                "Pilih Dealer tujuan.",
                "Pilih Peran (Admin, Sales, atau Cashier).",
                "Isi Nama Lengkap, Email, dan Kata Sandi.",
                "Klik Buat Pengguna.",
            ],
            Fields =
            [
                R("Dealer", "Dealer tempat pengguna ditugaskan.", "Wajib dipilih"),
                R("Peran", "Peran pengguna: Admin, Sales, atau Cashier.", "Wajib dipilih"),
                R("Nama Lengkap", "Nama pengguna.", "Wajib diisi", "Maksimal 200 karakter"),
                R("Email", "Email untuk login.", "Wajib diisi", "Format email valid",
                  "Harus unik"),
                R("Kata Sandi", "Kata sandi awal pengguna.",
                  "Wajib diisi", "Minimal 8 karakter", "Mengandung huruf besar dan angka"),
            ],
            Notes = "Daftarkan dealer terlebih dahulu sebelum membuat pengguna.",
        });

        // ── Administrasi ▸ Staf ──────────────────────────────────────────────────
        l.Add(new PageGuide
        {
            Route = "/dealer/users",
            Title = "Staf",
            Purpose = "Membuat akun staf (Sales / Kasir) untuk dealer Anda.",
            Access = "Admin dealer",
            Steps =
            [
                "Pilih Peran (Sales atau Cashier).",
                "Isi Nama Lengkap, Email, dan Kata Sandi.",
                "Klik tombol simpan untuk membuat akun.",
            ],
            Fields =
            [
                R("Peran", "Peran staf: Sales atau Cashier.", "Wajib dipilih"),
                R("Nama Lengkap", "Nama staf.", "Wajib diisi", "Maksimal 200 karakter"),
                R("Email", "Email untuk login staf.", "Wajib diisi", "Format email valid",
                  "Harus unik"),
                R("Kata Sandi", "Kata sandi awal staf.",
                  "Wajib diisi", "Minimal 8 karakter", "Mengandung huruf besar dan angka"),
            ],
        });
    }
}

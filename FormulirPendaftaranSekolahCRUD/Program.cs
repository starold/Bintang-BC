using Microsoft.EntityFrameworkCore;
using FormulirPendaftaranSekolahCRUD.Helper;
using FormulirPendaftaranSekolahCRUD.Enums;
using FormulirPendaftaranSekolahCRUD.Model;
using FormulirPendaftaranSekolahCRUD.Controller;

// Setup EF Core dengan SQLite
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite("Data Source=sekolah.db")
    .Options;

using var context = new AppDbContext(options);
context.Database.EnsureCreated();

var studentController = new StudentController(context);
var kelasController = new KelasController(context);

Console.WriteLine("==========================================");
Console.WriteLine("  FORMULIR PENDAFTARAN SEKOLAH - CRUD");
Console.WriteLine("==========================================");

bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("========== MENU UTAMA ==========");
    Console.WriteLine("1. Lihat Semua Siswa");
    Console.WriteLine("2. Tambah Siswa Baru");
    Console.WriteLine("3. Update Data Siswa");
    Console.WriteLine("4. Hapus Siswa");
    Console.WriteLine("5. Lihat Daftar Kelas");
    Console.WriteLine("6. Keluar");
    Console.WriteLine("================================");
    Console.Write("Pilih menu (1-6): ");

    var pilihan = Console.ReadLine()?.Trim();

    var menuActions = new Dictionary<string, Action>
    {
        { "1", LihatSemuaSiswa },
        { "2", TambahSiswa },
        { "3", UpdateSiswa },
        { "4", HapusSiswa },
        { "5", LihatDaftarKelas },
        { "6", () => { 
            running = false; 
            Console.WriteLine("\nTerima kasih! Sampai jumpa."); 
        }}
    };

    if (pilihan != null && menuActions.TryGetValue(pilihan, out var action))
    {
        action();
    }
    else
    {
        Console.WriteLine("\n[!] Pilihan tidak valid. Silakan pilih 1-6.");
    }

}

// ========== FUNGSI-FUNGSI MENU ==========

void LihatSemuaSiswa()
{
    Console.WriteLine("\n--- DAFTAR SEMUA SISWA ---");
    var students = studentController.GetAllStudents();

    if (students.Count == 0)
    {
        Console.WriteLine("Belum ada data siswa.");
        return;
    }

    for (int i = 0; i < students.Count; i++)
    {
        var s = students[i];
        Console.WriteLine($"\n  [{i + 1}] {s.NamaLengkap}");
        Console.WriteLine($"Alamat        : {s.Alamat}");
        Console.WriteLine($"Gender        : {s.Gender}");
        Console.WriteLine($"Tanggal Lahir : {s.TanggalLahir:dd-MM-yyyy}");
        Console.WriteLine($"Orang Tua     : {s.NamaOrangTua}");
        Console.WriteLine($"Kelas         : {s.Kelas?.NamaKelas ?? "-"}");
        Console.WriteLine($"Didaftarkan   : {s.CreatedAt:dd-MM-yyyy HH:mm}");
    }

    Console.WriteLine($"\nTotal: {students.Count} siswa");
}

void TambahSiswa()
{
    Console.WriteLine("\n--- TAMBAH SISWA BARU ---");

    Console.Write("Nama Lengkap    : ");
    var nama = Console.ReadLine()?.Trim() ?? "";

    Console.Write("Alamat          : ");
    var alamat = Console.ReadLine()?.Trim() ?? "";

    Console.WriteLine("Gender (1 = Laki-Laki, 2 = Perempuan): ");
    var genderInput = Console.ReadLine()?.Trim();
    GenderType gender = genderInput == "2" ? GenderType.Perempuan : GenderType.LakiLaki;

    Console.Write("Tanggal Lahir (dd-MM-yyyy) : ");
    var tglInput = Console.ReadLine()?.Trim() ?? "";
    if (!DateTime.TryParseExact(tglInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var tanggalLahir))
    {
        Console.WriteLine("[!] Format tanggal tidak valid. Gunakan dd-MM-yyyy");
        return;
    }

    Console.Write("Nama Orang Tua  : ");
    var orangTua = Console.ReadLine()?.Trim() ?? "";

    // Pilih Kelas
    Console.WriteLine("\nPilih Kelas:");
    var kelasList = kelasController.GetAllKelas();

    foreach (var k in kelasList)
    {
        Console.WriteLine($"  {k.Id}. {k.NamaKelas} - {k.Deskripsi}");
    }

    Console.Write("Pilih kelas (nomor): ");
    var kelasInput = Console.ReadLine()?.Trim();
    if (!int.TryParse(kelasInput, out var kelasId))
    {
        Console.WriteLine("[!] Input kelas tidak valid.");
        return;
    }

    var student = new Student
    {
        NamaLengkap = nama,
        Alamat = alamat,
        Gender = gender,
        TanggalLahir = tanggalLahir,
        NamaOrangTua = orangTua,
        KelasId = kelasId
    };

    var result = studentController.CreateStudent(student);

    if (result != null)
    {
        Console.WriteLine($"\n[OK] Siswa '{result.NamaLengkap}' berhasil ditambahkan ke kelas {result.Kelas?.NamaKelas}!");
    }
    else
    {
        Console.WriteLine("\n[ERROR] Gagal menambah siswa. Pastikan kelas yang dipilih valid.");
    }
}

void UpdateSiswa()
{
    Console.WriteLine("\n--- UPDATE DATA SISWA ---");

    // Tampilkan daftar dulu
    var students = studentController.GetAllStudents();

    if (students.Count == 0)
    {
        Console.WriteLine("Belum ada data siswa.");
        return;
    }

    for (int i = 0; i < students.Count; i++)
    {
        Console.WriteLine($"  [{i + 1}] {students[i].NamaLengkap} - Kelas {students[i].Kelas?.NamaKelas}");
    }

    Console.Write("\nPilih nomor siswa yang ingin diupdate: ");
    var indexInput = Console.ReadLine()?.Trim();
    if (!int.TryParse(indexInput, out var nomor) || nomor < 1 || nomor > students.Count)
    {
        Console.WriteLine("[!] Nomor tidak valid.");
        return;
    }

    var existing = students[nomor - 1];
    Console.WriteLine($"\nMengupdate data: {existing.NamaLengkap}");
    Console.WriteLine("(Kosongkan untuk tetap dengan data lama)\n");

    Console.Write($"Nama Lengkap [{existing.NamaLengkap}]: ");
    var nama = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(nama)) nama = existing.NamaLengkap;

    Console.Write($"Alamat [{existing.Alamat}]: ");
    var alamat = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(alamat)) alamat = existing.Alamat;

    Console.Write($"Gender (1=Laki-Laki, 2=Perempuan) [{existing.Gender}]: ");
    var genderInput = Console.ReadLine()?.Trim();
    GenderType genderUpdate = existing.Gender;
    if (genderInput == "1") genderUpdate = GenderType.LakiLaki;
    else if (genderInput == "2") genderUpdate = GenderType.Perempuan;

    Console.Write($"Tanggal Lahir (dd-MM-yyyy) [{existing.TanggalLahir:dd-MM-yyyy}]: ");
    var tglInput = Console.ReadLine()?.Trim();
    DateTime tanggalLahir = existing.TanggalLahir;
    if (!string.IsNullOrEmpty(tglInput))
    {
        if (!DateTime.TryParseExact(tglInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out tanggalLahir))
        {
            Console.WriteLine("[!] Format tanggal tidak valid. Data tidak diupdate.");
            return;
        }
    }

    Console.Write($"Nama Orang Tua [{existing.NamaOrangTua}]: ");
    var orangTua = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(orangTua)) orangTua = existing.NamaOrangTua;

    // Pilih Kelas
    Console.WriteLine($"\nKelas saat ini: {existing.Kelas?.NamaKelas}");
    var kelasList = kelasController.GetAllKelas();

    foreach (var k in kelasList)
    {
        Console.WriteLine($"  {k.Id}. {k.NamaKelas} - {k.Deskripsi}");
    }

    Console.Write($"Pilih kelas (nomor) [{existing.KelasId}]: ");
    var kelasInput = Console.ReadLine()?.Trim();
    int kelasId = existing.KelasId;
    if (!string.IsNullOrEmpty(kelasInput) && int.TryParse(kelasInput, out var newKelasId))
    {
        kelasId = newKelasId;
    }

    var updatedData = new Student
    {
        NamaLengkap = nama,
        Alamat = alamat,
        Gender = genderUpdate,
        TanggalLahir = tanggalLahir,
        NamaOrangTua = orangTua,
        KelasId = kelasId
    };

    var result = studentController.UpdateStudent(nomor - 1, updatedData);

    if (result != null)
    {
        Console.WriteLine($"\n[OK] Data siswa '{result.NamaLengkap}' berhasil diupdate!");
    }
    else
    {
        Console.WriteLine("\n[ERROR] Gagal mengupdate data siswa.");
    }
}

void HapusSiswa()
{
    Console.WriteLine("\n--- HAPUS SISWA ---");

    var students = studentController.GetAllStudents();

    if (students.Count == 0)
    {
        Console.WriteLine("Belum ada data siswa.");
        return;
    }

    for (int i = 0; i < students.Count; i++)
    {
        Console.WriteLine($"  [{i + 1}] {students[i].NamaLengkap} - Kelas {students[i].Kelas?.NamaKelas}");
    }

    Console.Write("\nPilih nomor siswa yang ingin dihapus: ");
    var indexInput = Console.ReadLine()?.Trim();
    if (!int.TryParse(indexInput, out var nomor) || nomor < 1 || nomor > students.Count)
    {
        Console.WriteLine("[!] Nomor tidak valid.");
        return;
    }

    Console.Write($"Yakin ingin menghapus '{students[nomor - 1].NamaLengkap}'? (y/n): ");
    var konfirmasi = Console.ReadLine()?.Trim().ToLower();

    if (konfirmasi != "y")
    {
        Console.WriteLine("Hapus dibatalkan.");
        return;
    }

    var success = studentController.DeleteStudent(nomor - 1);

    if (success)
    {
        Console.WriteLine("\n[OK] Siswa berhasil dihapus!");
    }
    else
    {
        Console.WriteLine("\n[ERROR] Gagal menghapus siswa.");
    }
}

void LihatDaftarKelas()
{
    Console.WriteLine("\n--- DAFTAR KELAS ---");

    var kelasList = kelasController.GetAllKelas();

    foreach (var k in kelasList)
    {
        Console.WriteLine($"\n  [{k.Id}] {k.NamaKelas}");
        Console.WriteLine($"      Deskripsi    : {k.Deskripsi ?? "-"}");
        Console.WriteLine($"      Jumlah Siswa : {k.Students?.Count ?? 0}");
    }
}

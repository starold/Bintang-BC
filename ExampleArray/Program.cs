namespace BasicCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string nama = "Bintang";
            int umur = 22;
            double ipk = 3.45;
            bool isActive = true;

            int [] nilai={80,85,90};
            int total = 0;
            for (int i=0; i < nilai.Length; i++)
            {
                total += nilai[i];
            }
            double rataRata = (double)total / nilai.Length;
            bool lulus = rataRata >= 75;
            string? catatan = null;
            string hasilCatatan = catatan ?? "Tidak ada catatan";;
            Console.WriteLine("Nama Mahasiswa: " + nama);
            Console.WriteLine("Umur: " + umur);
            Console.WriteLine("IPK: " + ipk);
            Console.WriteLine("Status Aktif: " + isActive); 
            Console.WriteLine("Rata-rata Nilai: " + rataRata);
            Console.WriteLine("Catatan " + hasilCatatan);
            if (lulus)
            {
                Console.WriteLine("Selamat, Anda lulus!");
            }
            else
            {
                Console.WriteLine("Maaf, Anda tidak lulus.");
            }
        }
    }
}
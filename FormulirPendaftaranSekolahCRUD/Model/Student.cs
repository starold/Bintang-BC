using System.ComponentModel.DataAnnotations;
using FormulirPendaftaranSekolahCRUD.Enums;
using FormulirPendaftaranSekolahCRUD.Interfaces;

namespace FormulirPendaftaranSekolahCRUD.Model
{
    public class Student : IStudent
    {
        [Required]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required]
        public string Alamat { get; set; } = string.Empty;

        public GenderType Gender { get; set; }

        public DateTime TanggalLahir { get; set; }

        [Required]
        public string NamaOrangTua { get; set; } = string.Empty;

        // FK ke Kelas
        public int KelasId { get; set; }

        // Navigation property
        public Kelas Kelas { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

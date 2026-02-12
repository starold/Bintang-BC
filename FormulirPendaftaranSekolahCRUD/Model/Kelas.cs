using System.ComponentModel.DataAnnotations;
using FormulirPendaftaranSekolahCRUD.Interfaces;

namespace FormulirPendaftaranSekolahCRUD.Model
{
    public class Kelas : IKelas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NamaKelas { get; set; } = string.Empty;

        public string? Deskripsi { get; set; }

        // Navigation property
        public List<Student> Students { get; set; } = new();
    }
}

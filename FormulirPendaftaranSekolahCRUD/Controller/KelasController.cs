using FormulirPendaftaranSekolahCRUD.Helper;
using FormulirPendaftaranSekolahCRUD.Model;
using Microsoft.EntityFrameworkCore;

namespace FormulirPendaftaranSekolahCRUD.Controller
{
    public class KelasController
    {
        private readonly AppDbContext _context;

        public KelasController(AppDbContext context)
        {
            _context = context;
        }

        public List<Kelas> GetAllKelas()
        {
            return _context.Kelass.Include(k => k.Students).ToList();
        }

        public Kelas? GetKelasById(int id)
        {
            return _context.Kelass.Include(k => k.Students).FirstOrDefault(k => k.Id == id);
        }
    }
}

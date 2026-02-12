using FormulirPendaftaranSekolahCRUD.Helper;
using FormulirPendaftaranSekolahCRUD.Model;
using Microsoft.EntityFrameworkCore;

namespace FormulirPendaftaranSekolahCRUD.Controller
{
    public class StudentController
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        public List<Student> GetAllStudents()
        {
            return _context.Students.Include(s => s.Kelas).ToList();
        }

        public Student? GetStudentByIndex(int index)
        {
            var students = _context.Students.Include(s => s.Kelas).ToList();

            if (index < 0 || index >= students.Count)
                return null;

            return students[index];
        }

        public Student? CreateStudent(Student student)
        {
            var kelas = _context.Kelass.Find(student.KelasId);
            if (kelas == null)
                return null;

            _context.Students.Add(student);
            _context.SaveChanges();

            // Reload with navigation
            _context.Entry(student).Reference(s => s.Kelas).Load();

            return student;
        }

        public Student? UpdateStudent(int index, Student updatedData)
        {
            var students = _context.Students.Include(s => s.Kelas).ToList();

            if (index < 0 || index >= students.Count)
                return null;

            var student = students[index];

            // Update fields
            student.NamaLengkap = updatedData.NamaLengkap;
            student.Alamat = updatedData.Alamat;
            student.Gender = updatedData.Gender;
            student.TanggalLahir = updatedData.TanggalLahir;
            student.NamaOrangTua = updatedData.NamaOrangTua;
            student.KelasId = updatedData.KelasId;

            _context.SaveChanges();

            // Reload navigation
            _context.Entry(student).Reference(s => s.Kelas).Load();

            return student;
        }

        public bool DeleteStudent(int index)
        {
            var students = _context.Students.ToList();

            if (index < 0 || index >= students.Count)
                return false;

            var student = students[index];
            _context.Students.Remove(student);
            _context.SaveChanges();

            return true;
        }
    }
}

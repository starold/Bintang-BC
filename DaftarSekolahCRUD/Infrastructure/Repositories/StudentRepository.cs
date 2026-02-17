using DaftarSekolahCRUD.Domain.Entities;
using DaftarSekolahCRUD.Domain.Repositories;
using DaftarSekolahCRUD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DaftarSekolahCRUD.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.Profile)
                .Include(s => s.StudentExtracurriculars)
                    .ThenInclude(se => se.Extracurricular)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(Guid id)
        {
            return await _context.Students
                .Include(s => s.ClassRoom)
                .Include(s => s.Profile)
                .Include(s => s.StudentExtracurriculars)
                    .ThenInclude(se => se.Extracurricular)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            var existingExtracurriculars = await _context.StudentExtracurriculars
                .Where(se => se.StudentId == student.Id)
                .ToListAsync();
            _context.StudentExtracurriculars.RemoveRange(existingExtracurriculars);

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _context.Students
                .Include(s => s.Profile)
                .Include(s => s.StudentExtracurriculars)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

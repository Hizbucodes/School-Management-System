using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Student?> GetByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber, cancellationToken);
        }

        public async Task<Student?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.IdentityUserId == identityUserId, cancellationToken);
        }

        public async Task<IEnumerable<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.Class)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Student>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .Include(s => s.Class)
                .Where(s => s.ClassId == classId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Student> CreateAsync(Student student, CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return student;
        }

        public async Task<Student> UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync(cancellationToken);
            return student;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var student = await _context.Students.FindAsync(new object[] { id }, cancellationToken);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Students.AnyAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Students.AnyAsync(s => s.AdmissionNumber == admissionNumber, cancellationToken);
        }
    }
}

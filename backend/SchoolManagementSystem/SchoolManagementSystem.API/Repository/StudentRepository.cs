using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Helpers;
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

        public async Task<PagedList<Student>> GetAllAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
        {
            var query = _context.Students
                .Include(s => s.Class)
                .AsNoTracking();

            // 2. Apply Filtering (Search)
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var search = parameters.SearchTerm.ToLower();
                query = query.Where(s =>
                    s.AdmissionNumber.ToLower().Contains(search));
            }

            // 3. Apply Sorting
            query = parameters.SortBy switch
            {
                "AdmissionNumber" => parameters.IsDescending
                    ? query.OrderByDescending(s => s.AdmissionNumber)
                    : query.OrderBy(s => s.AdmissionNumber),
                "EnrollmentDate" => parameters.IsDescending
                    ? query.OrderByDescending(s => s.EnrollmentDate)
                    : query.OrderBy(s => s.EnrollmentDate),
                _ => query.OrderBy(s => s.Id)
            };

         
            return await PagedList<Student>.CreateAsync(query, parameters.PageNumber, parameters.PageSize);
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

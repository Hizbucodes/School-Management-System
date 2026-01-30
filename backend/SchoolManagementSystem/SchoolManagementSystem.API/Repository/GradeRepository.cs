using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.API.Repository
{
    public class GradeRepository : IGradeRepository
    {
        private readonly ApplicationDbContext _context;

        public GradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Grade>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .ToListAsync(cancellationToken);
        }

        public async Task<Grade> CreateAsync(Grade grade, CancellationToken cancellationToken = default)
        {
            await _context.Grades.AddAsync(grade, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return grade;
        }

        public async Task<Grade> UpdateAsync(Grade grade, CancellationToken cancellationToken = default)
        {
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync(cancellationToken);
            return grade;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var grade = await _context.Grades.FindAsync(new object[] { id }, cancellationToken);
            if (grade == null)
                return false;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Grades.AnyAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Where(g => g.StudentId == studentId)
                .OrderByDescending(g => g.Score)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Grade>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Where(g => g.CourseId == courseId)
                .OrderByDescending(g => g.Score)
                .ToListAsync(cancellationToken);
        }

        public async Task<Grade?> GetStudentCourseGradeAsync(
            Guid studentId,
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.StudentId == studentId && g.CourseId == courseId, cancellationToken);
        }

        public async Task<bool> ExistsForStudentCourseAsync(
            Guid studentId,
            Guid courseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .AnyAsync(g => g.StudentId == studentId && g.CourseId == courseId, cancellationToken);
        }

        public async Task CreateMultipleAsync(
            IEnumerable<Grade> grades,
            CancellationToken cancellationToken = default)
        {
            await _context.Grades.AddRangeAsync(grades, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesWithCoursesAsync(
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Include(g => g.Student)
                .Where(g => g.StudentId == studentId)
                .OrderBy(g => g.Course.Name)
                .ToListAsync(cancellationToken);
        }
    }
}

using SchoolManagementSystem.API.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Exam?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Exam>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Exam> CreateAsync(Exam exam, CancellationToken cancellationToken = default)
        {
            await _context.Exams.AddAsync(exam, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return exam;
        }

        public async Task<Exam> UpdateAsync(Exam exam, CancellationToken cancellationToken = default)
        {
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync(cancellationToken);
            return exam;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exam = await _context.Exams.FindAsync(new object[] { id }, cancellationToken);
            if (exam == null)
                return false;

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Exam>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Where(e => e.CourseId == courseId)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Exam>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Where(e => e.ClassId == classId)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Exam>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Where(e => e.ExamDate >= startDate && e.ExamDate <= endDate)
                .OrderBy(e => e.ExamDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Exam>> GetUpcomingExamsAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.Today;
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Where(e => e.ExamDate >= today)
                .OrderBy(e => e.ExamDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<Exam?> GetExamWithStudentsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Class)
                .Include(e => e.StudentExams)
                    .ThenInclude(se => se.Student)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsForCourseOnDateAsync(
            Guid courseId,
            DateTime examDate,
            CancellationToken cancellationToken = default)
        {
            var dateOnly = examDate.Date;
            return await _context.Exams
                .AnyAsync(e => e.CourseId == courseId && e.ExamDate.Date == dateOnly, cancellationToken);
        }
    }
}

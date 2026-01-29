using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public async Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.Course)
                .ToListAsync(cancellationToken);
        }

        public async Task<Attendance> CreateAsync(Attendance attendance, CancellationToken cancellationToken = default)
        {
            await _context.Attendances.AddAsync(attendance, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return attendance;
        }

        public async Task<Attendance> UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync(cancellationToken);
            return attendance;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var attendance = await _context.Attendances.FindAsync(new object[] { id }, cancellationToken);
            if (attendance == null)
                return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Attendances.AnyAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Attendances
                .Include(a => a.Class)
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.ClassId == classId)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.Course)
                .Where(a => a.Date.Date == dateOnly)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByClassAndDateAsync(
            Guid classId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.ClassId == classId && a.Date.Date == dateOnly)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Attendance>> GetByCourseAndDateAsync(
            Guid courseId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Where(a => a.CourseId == courseId && a.Date.Date == dateOnly)
                .ToListAsync(cancellationToken);
        }

        public async Task<Attendance?> GetStudentAttendanceAsync(
            Guid studentId,
            Guid courseId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Class)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.StudentId == studentId
                    && a.CourseId == courseId
                    && a.Date.Date == dateOnly, cancellationToken);
        }

        public async Task<bool> AttendanceExistsAsync(
            Guid studentId,
            Guid courseId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .AnyAsync(a => a.StudentId == studentId
                    && a.CourseId == courseId
                    && a.Date.Date == dateOnly, cancellationToken);
        }

        public async Task CreateMultipleAsync(
            IEnumerable<Attendance> attendances,
            CancellationToken cancellationToken = default)
        {
            await _context.Attendances.AddRangeAsync(attendances, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

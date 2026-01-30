using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.API.Data;
using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public class TimeTableRepository : ITimeTableRepository
    {
        private readonly ApplicationDbContext _context;

        public TimeTableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TimeTable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<TimeTable> CreateAsync(TimeTable timeTable, CancellationToken cancellationToken = default)
        {
            await _context.TimeTables.AddAsync(timeTable, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return timeTable;
        }

        public async Task<TimeTable> UpdateAsync(TimeTable timeTable, CancellationToken cancellationToken = default)
        {
            _context.TimeTables.Update(timeTable);
            await _context.SaveChangesAsync(cancellationToken);
            return timeTable;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var timeTable = await _context.TimeTables.FindAsync(new object[] { id }, cancellationToken);
            if (timeTable == null)
                return false;

            _context.TimeTables.Remove(timeTable);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables.AnyAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Where(t => t.ClassId == classId)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Course)
                .Where(t => t.TeacherId == teacherId)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Teacher)
                .Where(t => t.CourseId == courseId)
                .OrderBy(t => t.Day)
                .ThenBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetByClassAndDayAsync(
            Guid classId,
            DayOfWeek day,
            CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Course)
                .Include(t => t.Teacher)
                .Where(t => t.ClassId == classId && t.Day == day)
                .OrderBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TimeTable>> GetByTeacherAndDayAsync(
            Guid teacherId,
            DayOfWeek day,
            CancellationToken cancellationToken = default)
        {
            return await _context.TimeTables
                .Include(t => t.Class)
                .Include(t => t.Course)
                .Where(t => t.TeacherId == teacherId && t.Day == day)
                .OrderBy(t => t.StartTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasConflictAsync(
            Guid classId,
            DayOfWeek day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.TimeTables
                .Where(t => t.ClassId == classId && t.Day == day);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync(t =>
                (startTime >= t.StartTime && startTime < t.EndTime) ||
                (endTime > t.StartTime && endTime <= t.EndTime) ||
                (startTime <= t.StartTime && endTime >= t.EndTime),
                cancellationToken);
        }

        public async Task<bool> TeacherHasConflictAsync(
            Guid teacherId,
            DayOfWeek day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.TimeTables
                .Where(t => t.TeacherId == teacherId && t.Day == day);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync(t =>
                (startTime >= t.StartTime && startTime < t.EndTime) ||
                (endTime > t.StartTime && endTime <= t.EndTime) ||
                (startTime <= t.StartTime && endTime >= t.EndTime),
                cancellationToken);
        }

        public async Task<bool> RoomHasConflictAsync(
            string roomNumber,
            DayOfWeek day,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.TimeTables
                .Where(t => t.RoomNumber == roomNumber && t.Day == day);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync(t =>
                (startTime >= t.StartTime && startTime < t.EndTime) ||
                (endTime > t.StartTime && endTime <= t.EndTime) ||
                (startTime <= t.StartTime && endTime >= t.EndTime),
                cancellationToken);
        }

        public async Task CreateMultipleAsync(
            IEnumerable<TimeTable> timeTables,
            CancellationToken cancellationToken = default)
        {
            await _context.TimeTables.AddRangeAsync(timeTables, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

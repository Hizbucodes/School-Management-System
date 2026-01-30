using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface ITimeTableRepository
    {
        Task<TimeTable?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTable>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TimeTable> CreateAsync(TimeTable timeTable, CancellationToken cancellationToken = default);
        Task<TimeTable> UpdateAsync(TimeTable timeTable, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);


        Task<IEnumerable<TimeTable>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTable>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTable>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTable>> GetByClassAndDayAsync(Guid classId, DayOfWeek day, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeTable>> GetByTeacherAndDayAsync(Guid teacherId, DayOfWeek day, CancellationToken cancellationToken = default);
        Task<bool> HasConflictAsync(Guid classId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime, Guid? excludeId, CancellationToken cancellationToken = default);
        Task<bool> TeacherHasConflictAsync(Guid teacherId, DayOfWeek day, TimeSpan startTime, TimeSpan endTime, Guid? excludeId, CancellationToken cancellationToken = default);
        Task<bool> RoomHasConflictAsync(string roomNumber, DayOfWeek day, TimeSpan startTime, TimeSpan endTime, Guid? excludeId, CancellationToken cancellationToken = default);
        Task CreateMultipleAsync(IEnumerable<TimeTable> timeTables, CancellationToken cancellationToken = default);
    }
}

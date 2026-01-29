using SchoolManagementSystem.API.Models;

namespace SchoolManagementSystem.API.Repository
{
    public interface IAttendanceRepository
    {
        Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Attendance> CreateAsync(Attendance attendance, CancellationToken cancellationToken = default);
        Task<Attendance> UpdateAsync(Attendance attendance, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    
        Task<IEnumerable<Attendance>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetByClassIdAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetByClassAndDateAsync(Guid classId, DateTime date, CancellationToken cancellationToken = default);
        Task<IEnumerable<Attendance>> GetByCourseAndDateAsync(Guid courseId, DateTime date, CancellationToken cancellationToken = default);
        Task<Attendance?> GetStudentAttendanceAsync(Guid studentId, Guid courseId, DateTime date, CancellationToken cancellationToken = default);
        Task<bool> AttendanceExistsAsync(Guid studentId, Guid courseId, DateTime date, CancellationToken cancellationToken = default);
        Task CreateMultipleAsync(IEnumerable<Attendance> attendances, CancellationToken cancellationToken = default);
    }
}

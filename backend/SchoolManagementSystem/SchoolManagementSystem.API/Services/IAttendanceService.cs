using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface IAttendanceService
    {
        Task<(bool Succeeded, string Message, Guid? AttendanceId)> CreateAttendanceAsync(AttendanceCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateAttendanceAsync(Guid id, AttendanceUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteAttendanceAsync(Guid id, CancellationToken cancellationToken = default);


        Task<(bool Succeeded, string Message, int RecordsCreated)> MarkBulkAttendanceAsync(BulkAttendanceCreateDto dto, CancellationToken cancellationToken = default);

 
        Task<AttendanceResponseDto?> GetAttendanceByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<AttendanceResponseDto>> GetAllAttendanceAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<AttendanceResponseDto>> GetStudentAttendanceAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AttendanceResponseDto>> GetClassAttendanceAsync(Guid classId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AttendanceResponseDto>> GetCourseAttendanceAsync(Guid courseId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AttendanceResponseDto>> GetAttendanceByDateAsync(DateTime date, CancellationToken cancellationToken = default);


        Task<AttendanceReportDto?> GetCourseAttendanceReportAsync(Guid courseId, DateTime date, CancellationToken cancellationToken = default);
        Task<AttendanceStatsDto?> GetStudentAttendanceStatsAsync(Guid studentId, Guid? courseId, CancellationToken cancellationToken = default);
    }
}

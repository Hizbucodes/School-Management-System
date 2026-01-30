using SchoolManagementSystem.API.Dtos;

namespace SchoolManagementSystem.API.Services
{
    public interface IGradeService
    {
        Task<(bool Succeeded, string Message, Guid? GradeId)> CreateGradeAsync(GradeCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message, int GradesCreated)> CreateBulkGradesAsync(BulkGradeCreateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> UpdateGradeAsync(Guid id, GradeUpdateDto dto, CancellationToken cancellationToken = default);
        Task<(bool Succeeded, string Message)> DeleteGradeAsync(Guid id, CancellationToken cancellationToken = default);


        Task<GradeResponseDto?> GetGradeByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<GradeResponseDto>> GetAllGradesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<GradeResponseDto>> GetStudentGradesAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<GradeResponseDto>> GetCourseGradesAsync(Guid courseId, CancellationToken cancellationToken = default);

  
        Task<StudentReportCardDto?> GetStudentReportCardAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<CourseGradeStatsDto?> GetCourseGradeStatsAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
